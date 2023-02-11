using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using Application.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

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
    public class TokenController : ControllerBase
    {
        private readonly ILogger<TokenController> _logger;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;
        private readonly AuthorizationService _authorizationService;

        /// <inheritdoc />
        public TokenController(AuthorizationService authorizationService, ILogger<TokenController> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
            _mapper = new MapperConfiguration(cfg =>
                cfg.CreateMap<Models.RegisterUser, Domain.Entities.ApplicationUser>()).CreateMapper();
            _authorizationService = authorizationService;
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
            if(await Task.Run(() => _authorizationService.CheckUserExist(user.username)))
                return (false, "Username is already taken");
            return string.IsNullOrWhiteSpace(user.email) ? (false, "Email is required") : (true, null);
        }

        /// <summary>
        /// Đăng ký
        /// </summary>
        /// <param name="user">Đối tượng người dùng</param>
        /// <returns></returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/v1/Token/register
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
        [HttpPost("register")]
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
        ///     POST /api/v1/Token/getToken
        ///     {
        ///         "username": "string",
        ///         "password": "string"
        ///     }
        /// </remarks>
        /// <response code="200">Lấy token thành công</response>
        /// <response code="400">Lỗi dữ liệu đầu vào</response>
        /// <response code="429">Request quá nhiều</response>
        [HttpPost("getToken")]
        [AllowAnonymous]
        public async Task<IActionResult> GetToken([FromBody] Models.LoginUser loginUser)
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
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, _config.GetSection("Jwt:Subject").Value ?? throw new InvalidOperationException()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds().ToString()),
                new Claim("UserName", loginUser.username)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("Jwt:Key").Value ?? throw new InvalidOperationException()));

            var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            var token = new JwtSecurityToken(
                _config.GetSection("Jwt:Issuer").Value,
                _config.GetSection("Jwt:Audience").Value,
                claims,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: signIn
            );

            return Ok(new { status = true, expire = DateTime.UtcNow.AddDays(1), token = new  JwtSecurityTokenHandler().WriteToken(token) });
        }
    }
}
