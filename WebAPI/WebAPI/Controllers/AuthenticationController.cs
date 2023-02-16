using Application.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Application.Transaction;
using Microsoft.AspNetCore.Cors;

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
        private readonly AuthenticationService _authorizationService;
        private readonly TransactionService _transactionService;
        private readonly TokenValidationParameters _tokenValidationParameters;

        /// <inheritdoc />
        public AuthenticationController(
            AuthenticationService authorizationService,
            TransactionService transactionService,
            ILogger<AuthenticationController> logger,
            IConfiguration config,
            TokenValidationParameters tokenValidationParameters)
        {
            _logger = logger;
            _config = config;
            _mapper = new MapperConfiguration(cfg =>
                cfg.CreateMap<Models.RegisterUser, Domain.Entities.ApplicationUser>()).CreateMapper();
            _authorizationService = authorizationService;
            _transactionService = transactionService;
            _tokenValidationParameters = tokenValidationParameters;
        }

        private bool IsValidUser(Models.RegisterUser user, out string? message)
        {
            if (!string.IsNullOrWhiteSpace(user.email) &&
                !Regex.IsMatch(user.email, @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", RegexOptions.None,
                    TimeSpan.FromSeconds(2)))
            {
                message = "Email is invalid";
                return false;
            }

            if (!string.IsNullOrWhiteSpace(user.phoneNumber) &&
                !Regex.IsMatch(user.phoneNumber, @"^([0-9]{10})$", RegexOptions.None, TimeSpan.FromSeconds(2)))
            {
                message = "Phone number is invalid";
                return false;
            }

            if (string.IsNullOrWhiteSpace(user.username) || string.IsNullOrWhiteSpace(user.password))
            {
                message = "Username or password is required";
                return false;
            }

            if (!Regex.IsMatch(user.password, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$",
                    RegexOptions.None, TimeSpan.FromSeconds(2)))
            {
                message = "Password is too weak";
                return false;
            }

            if (_authorizationService.CheckUserExist(user.username))
            {
                message = "Username is already exists";
                return false;
            }

            if (string.IsNullOrWhiteSpace(user.email))
            {
                message = "Email is required";
                return false;
            }

            message = null;
            return true;
        }

        private Models.Auth GenerateJwtToken(Domain.Entities.ApplicationUser user)
        {
            var claims = new[]
            {
                new Claim("Id", user.Id),
                new Claim("UserName", user.UserName!),
                new Claim("Email", user.Email!),
                new Claim(JwtRegisteredClaimNames.Sub, _config.GetSection("Jwt:Subject").Value ?? throw new InvalidOperationException()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds().ToString())
            };

            var userClaims = _authorizationService.GetUserClaims(user.Id).ToArray();
            var allClaims = new Claim[claims.Length + userClaims.Length];
            Array.Copy(claims, allClaims, claims.Length);
            Array.Copy(userClaims, 0, allClaims, claims.Length, userClaims.Length);

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
                expiryDate = DateTime.UtcNow.AddDays(10),
                isRevoked = false,
                isUsed = false,
                userId = _authorizationService.GetUserId(user.UserName!)
            };

            _transactionService.ExecuteTransaction(() => { _authorizationService.AddToken(refreshToken); });

            return new Models.Auth()
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                refreshToken = refreshToken.token,
                isAuthSuccessful = true
            };

        }

        private Models.Auth VerifyAndGenerateToken(Models.TokenRefresh tokenRefresh)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();

            try
            {
                _tokenValidationParameters.ValidateLifetime = false;
                var principal = jwtTokenHandler.ValidateToken(tokenRefresh.token, _tokenValidationParameters, out var validatedToken);
                if (validatedToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256Signature, StringComparison.InvariantCultureIgnoreCase))
                    throw new SecurityTokenException("Invalid token");

                var expiryDateClaim = principal.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp);
                if (expiryDateClaim is null || !long.TryParse(expiryDateClaim.Value, out var utcExpiryDate))
                    return new Models.Auth { isAuthSuccessful = false, errors = new List<string> { "An error occurred" } };

                var expiryDateUtc = DateTimeOffset.FromUnixTimeSeconds(utcExpiryDate);
                if (expiryDateUtc.UtcDateTime > DateTime.UtcNow)
                    return new Models.Auth { isAuthSuccessful = false, errors = new List<string> { "Expired token" } };

                if (string.IsNullOrEmpty(tokenRefresh.refreshToken))
                    return new Models.Auth { isAuthSuccessful = false, errors = new List<string> { "Invalid tokens" } };
                var storedRefreshToken = _authorizationService.GetToken(tokenRefresh.refreshToken!);

                switch (storedRefreshToken)
                {
                    case { isUsed: true }:
                        return new Models.Auth { isAuthSuccessful = false, errors = new List<string> { "Token is used" } };
                    case { isRevoked: true }:
                        return new Models.Auth { isAuthSuccessful = false, errors = new List<string> { "Token is revoked" } };
                    case { jwtId: not null } when storedRefreshToken.jwtId != principal.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti)?.Value:
                        return new Models.Auth { isAuthSuccessful = false, errors = new List<string> { "Claim is invalid" } };
                    case { expiryDate: not null } when storedRefreshToken.expiryDate < DateTimeOffset.UtcNow:
                        return new Models.Auth { isAuthSuccessful = false, errors = new List<string> { "Token is expired" } };
                }

                storedRefreshToken.isUsed = true;
                _transactionService.ExecuteTransaction(() => { _authorizationService.UpdateRefreshToken(storedRefreshToken); });
                return GenerateJwtToken(_authorizationService.GetUserById(storedRefreshToken.userId!));
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
            var randomBytes = new byte[length];
            using (var rng = RandomNumberGenerator.Create())
                rng.GetBytes(randomBytes);
            var chars = new char[length];
            for (var i = 0; i < length; i++)
                chars[i] = CHARS[randomBytes[i] % CHARS.Length];
            return new string(chars);
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
        public ActionResult Register([FromBody] Models.RegisterUser user)
        {
            if (!IsValidUser(user, out var message))
                return BadRequest(new { status = false, message });

            try
            {
                var newUser = _mapper.Map<Domain.Entities.ApplicationUser>(user);
                newUser.Id = Guid.NewGuid().ToString();
                if (user.username != null) newUser.NormalizedUserName = user.username.ToUpper();
                if (user.email != null) newUser.NormalizedEmail = user.email.ToUpper();
                newUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.password);
                _transactionService.ExecuteTransaction(() => { _authorizationService.Register(newUser); });
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
        [EnableCors("AllowAll")]
        public ActionResult GetToken([FromBody] Models.LoginUser loginUser)
        {
            try
            {
                if (loginUser.username == null || loginUser.password == null)
                    return BadRequest(new { status = false, message = "Invalid client request" });
                if (_authorizationService.IsLockedAccount(loginUser.username))
                    return BadRequest(new { status = false, message = "Account is locked. Please contact admin" });
                if (!_authorizationService.CheckAccountValid(loginUser.username, loginUser.password))
                {
                   _transactionService.ExecuteTransaction(() => { _authorizationService.FailLogin(loginUser.username); });
                    return BadRequest(new { status = false, message = "Invalid credentials" });
                }
                _transactionService.ExecuteTransaction(() => { _authorizationService.ResetFailLogin(loginUser.username); });
                var jwtToken = GenerateJwtToken(_authorizationService.GetUserByUserName(loginUser.username));

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
        [EnableCors("AllowAll")]
        public ActionResult RefreshToken([FromBody] Models.TokenRefresh tokenRefresh)
        {
            try
            {
                if (tokenRefresh.refreshToken == null && tokenRefresh.token == null)
                    return BadRequest(new { status = false, message = "Invalid client request" });
                var result = VerifyAndGenerateToken(tokenRefresh);
                return Ok(new { status = true, result });
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while refreshing token");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { status = false, message = "An error occurred while processing your request" });
            }
        }
    }
}
