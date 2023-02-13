using Application.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;

namespace WebAPI.Controllers
{
    /**
    * @Project ASP.NET Core 7.0
    * @Author: Nguyen Xuan Nhan
    * @Team: 4FT
    * @Copyright (C) 2023 4FT. All rights reserved
    * @License MIT
    * @Create date Mon 23 Jan 2023 00:00:00 AM +07
    */

    /// <summary>
    /// Bảo mật API
    /// </summary>
    [Authorize]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly ILogger<AuthenticationController> _logger;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;
        private readonly AuthorizationService _authorizationService;
        private readonly TokenValidationParameters _tokenValidationParameters;

        /// <inheritdoc />
        public AuthenticationController(
            AuthorizationService authorizationService,
            ILogger<AuthenticationController> logger,
            IConfiguration config,
            TokenValidationParameters tokenValidationParameters)
        {
            _logger = logger;
            _config = config;
            _mapper = new MapperConfiguration(cfg =>
                cfg.CreateMap<Models.RegisterUser, Domain.Entities.ApplicationUser>()).CreateMapper();
            _authorizationService = authorizationService;
            _tokenValidationParameters = tokenValidationParameters;
        }

        private async Task<(bool, string?)> IsValidUser(Models.RegisterUser user)
        {
            if (!string.IsNullOrWhiteSpace(user.email) &&
                !Regex.IsMatch(user.email, @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", RegexOptions.None, TimeSpan.FromSeconds(2)))
                return (false, "Email is invalid");
            if (!string.IsNullOrWhiteSpace(user.phoneNumber) &&
                !Regex.IsMatch(user.phoneNumber, @"^([0-9]{10})$", RegexOptions.None, TimeSpan.FromSeconds(2)))
                return (false, "Phone is invalid");
            if (string.IsNullOrWhiteSpace(user.username) || string.IsNullOrWhiteSpace(user.password))
                return (false, "Username or password is invalid");
            if (!Regex.IsMatch(user.password, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$", RegexOptions.None, TimeSpan.FromSeconds(2)))
                return (false, "Password is too weak");
            if (await Task.Run(() => _authorizationService.CheckUserExist(user.username)))
                return (false, "Username is already taken");
            return string.IsNullOrWhiteSpace(user.email) ? (false, "Email is required") : (true, null);
        }

        private async Task<Models.Auth> GenerateJwtToken(Domain.Entities.ApplicationUser user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, _config.GetSection("Jwt:Subject").Value ?? throw new InvalidOperationException()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds().ToString()),
                new Claim("UserName", user.UserName!)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("Jwt:Key").Value ?? throw new InvalidOperationException()));

            var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            var token = new JwtSecurityToken(
                _config.GetSection("Jwt:Issuer").Value,
                _config.GetSection("Jwt:Audience").Value,
                claims,
                expires: DateTime.UtcNow.Add(TimeSpan.Parse(_config.GetSection("Jwt:ExpiryTimeFrame").Value ?? throw new InvalidOperationException())),
                signingCredentials: signIn
                );


            var refreshToken = new Domain.Entities.RefreshToken
            {
                jwtId = token.Id,
                token = RandomStringGeneration(64),
                addedDate = DateTime.UtcNow,
                expiryDate = DateTime.UtcNow.AddMonths(6),
                isRevoked = false,
                isUsed = false,
                userId = await _authorizationService.GetUserId(user.UserName!)
            };

            await _authorizationService.AddToken(refreshToken);

            return new Models.Auth()
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                refreshToken = refreshToken.token,
                isAuthSuccessful = true
            };

        }

        private async Task<Models.Auth> VerifyAndGenerateToken(Models.TokenRefresh tokenRefresh)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();

            try
            {
                _tokenValidationParameters.ValidateLifetime = false;
                var principal = jwtTokenHandler.ValidateToken(tokenRefresh.token, _tokenValidationParameters, out var validatedToken);
                if (validatedToken is JwtSecurityToken jwtSecurityToken)
                {
                    var result = jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256Signature, StringComparison.InvariantCultureIgnoreCase);
                    if (result == false)
                        throw new SecurityTokenException("Invalid token");
                }
                var utcExpiryDate = long.Parse(principal.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp)!.Value);

                var expiryDateUtc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(utcExpiryDate)
                    .ToUniversalTime();

                if (expiryDateUtc > DateTime.UtcNow)
                    return new Models.Auth { isAuthSuccessful = false, errors = new List<string> { "Expired token" } };

                if (tokenRefresh.refreshToken == null)
                    return new Models.Auth { isAuthSuccessful = false, errors = new List<string> { "Invalid tokens" } };
                var storedRefreshToken = await _authorizationService.GetToken(tokenRefresh.refreshToken);

                if (storedRefreshToken.isUsed)
                    return new Models.Auth { isAuthSuccessful = false, errors = new List<string> { "Token is used" } };

                if (storedRefreshToken.isRevoked)
                    return new Models.Auth { isAuthSuccessful = false, errors = new List<string> { "Token is revoked" } };

                var jti = principal.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti)?.Value;

                if (storedRefreshToken.jwtId != jti)
                    return new Models.Auth { isAuthSuccessful = false, errors = new List<string> { "Unknown tokens" } };

                if (storedRefreshToken.expiryDate < DateTime.UtcNow)
                    return new Models.Auth
                    { isAuthSuccessful = false, errors = new List<string> { "Tokens is invalid" } };

                storedRefreshToken.isUsed = true;
                await _authorizationService.UpdateRefreshToken(storedRefreshToken);

                var dbUser = await _authorizationService.GetUserById(storedRefreshToken.userId!);

                return await GenerateJwtToken(dbUser);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Verify and generate token have an error");
                return new Models.Auth { isAuthSuccessful = false, errors = new List<string> { "Server error" } };
            }
        }

        private static string RandomStringGeneration(int length)
        {
            const string CHARS = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(CHARS, length)
                .Select(s => s[new Random().Next(s.Length)]).ToArray());
        }

        /// <summary>
        /// Đăng ký
        /// </summary>
        /// <param name="user">Đối tượng người dùng</param>
        /// <returns></returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/v1/Token/Register
        ///     {
        ///         "username": "string",
        ///         "password": "string",
        ///         "email": "string",
        ///         "phonenumber": "string"
        ///     }
        /// </remarks>
        /// <response code="200">Tạo tài khoản thành công</response>
        /// <response code="400">Lỗi dữ liệu đầu vào</response>
        /// <response code="429">Request quá nhiều</response>
        /// <response code="500">Lỗi server</response>
        [HttpPost("Register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] Models.RegisterUser user)
        {
            var (isValid, message) = await Task.Run(() => IsValidUser(user));
            if (!isValid)
                return BadRequest(new { status = false, message });
            try
            {
                var newUser = _mapper.Map<Domain.Entities.ApplicationUser>(user);
                newUser.Id = Guid.NewGuid().ToString();
                if (user.username != null) newUser.NormalizedUserName = user.username.ToUpper();
                if (user.email != null) newUser.NormalizedEmail = user.email.ToUpper();
                newUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.password);
                await Task.Run(() => _authorizationService.Register(newUser));
                return Ok(new { status = true, message = "Register successfully" });
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while registering");
                return StatusCode(StatusCodes.Status500InternalServerError, new { status = false, message = "An error occurred while processing your request" });
            }
        }


        /// <summary>
        /// Lấy bearer token
        /// </summary>
        /// <param name="loginUser">Đối tượng người dùng đăng nhập</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /api/v1/Token/Login
        ///     {
        ///         "username": "string",
        ///         "password": "string"
        ///     }
        /// </remarks>
        /// <response code="200">Lấy token thành công</response>
        /// <response code="400">Lỗi dữ liệu đầu vào</response>
        /// <response code="429">Request quá nhiều</response>
        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<IActionResult> GetToken([FromBody] Models.LoginUser loginUser)
        {
            try
            {
                if (loginUser.username == null || loginUser.password == null)
                    return BadRequest(new { status = false, message = "Invalid client request" });
                if (await Task.Run(() => _authorizationService.IsLockedAccount(loginUser.username)))
                    return BadRequest(new { status = false, message = "Account is locked. Please contact admin" });
                if (!await Task.Run(() => _authorizationService.CheckAccountValid(loginUser.username, loginUser.password)))
                {
                    await Task.Run(() => _authorizationService.FailLogin(loginUser.username));
                    return BadRequest(new { status = false, message = "Invalid credentials" });
                }
                await Task.Run(() => _authorizationService.ResetFailLogin(loginUser.username));

                var user = await Task.Run(() => _authorizationService.GetUserByUserName(loginUser.username));

                var jwtToken = await Task.Run(() => GenerateJwtToken(user));

                return Ok(new
                {
                    status = true,
                    jwtToken
                });
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while getting token");
                return StatusCode(StatusCodes.Status500InternalServerError, new { status = false, message = "An error occurred while processing your request" });
            }
        }

        /// <summary>
        /// Refresh token
        /// </summary>
        /// <param name="tokenRefresh">Đối tượng token</param>
        /// <returns></returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/v1/Token/RefreshToken
        ///     {
        ///         "token": "string",
        ///         "refreshToken": "string"
        ///     }
        /// </remarks>
        /// <response code="200">Refresh token thành công</response>
        /// <response code="429">Request quá nhiều</response>
        [HttpPost("RefreshToken")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshToken([FromBody] Models.TokenRefresh tokenRefresh)
        {
            if (tokenRefresh.refreshToken == null && tokenRefresh.token == null)
                return BadRequest(new { status = false, message = "Invalid client request" });
            var result = await Task.Run(() => VerifyAndGenerateToken(tokenRefresh));
            return Ok(new { status = true, result });
        }
    }
}
