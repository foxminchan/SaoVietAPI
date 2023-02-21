using Application.Services;
using Application.Transaction;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
    /// Phân quyền truy cập
    /// </summary>
    [Authorize]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AuthorizationController : ControllerBase
    {
        private readonly ILogger<AuthorizationController> _logger;
        private readonly IMapper _mapper;
        private readonly AuthorizationService _authorizationService;
        private readonly TransactionService _transactionService;

        /// <inheritdoc />
        public AuthorizationController(
            AuthorizationService authorizationService,
            TransactionService transactionService,
            ILogger<AuthorizationController> logger)
        {
            _logger = logger;
            _mapper = new MapperConfiguration(cfg =>
                cfg.CreateMap<Models.UserClaim, Domain.Entities.IdentityClaim>()).CreateMapper();
            _authorizationService = authorizationService;
            _transactionService = transactionService;
        }

        private bool IsValidClaim(Models.UserClaim claim, out string? message)
        {

            if (string.IsNullOrEmpty(claim.userId) ||
                string.IsNullOrEmpty(claim.claimValue) ||
                string.IsNullOrEmpty(claim.claimType))
            {
                message = "User id, claim value and claim type are require";
                return false;
            }

            if (!_authorizationService.IsExistsUser(claim.userId))
            {
                message = "User id is not exists";
                return false;
            }

            if (!_authorizationService.IsIdentityClaimExist(claim.userId, claim.claimType, claim.claimValue))
            {
                message = "Claim already exists";
                return false;
            }

            message = null;
            return true;
        }

        /// <summary>
        /// Gán quyền cho người dùng
        /// </summary>
        /// <param name="claim">Đối tượng quyền</param>
        /// <returns></returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/v1/Authorization
        ///     {
        ///         "userId": "string",
        ///         "claimType": "string",
        ///         "claimValue": "string"
        ///     }
        /// </remarks>
        /// <response code="200">Thêm claim thành công</response>
        /// <response code="400">Lỗi dữ liệu đầu vào</response>
        /// <response code="401">Không có quyền truy cập</response>
        /// <response code="408">Quá thời gian yêu cầu</response>
        /// <response code="429">Request quá nhiều</response>
        /// <response code="500">Lỗi server</response>
        [HttpPost]
        [Authorize(Policy = "Admin")]
        public IActionResult AddClaim([FromBody] Models.UserClaim claim)
        {
            if (!IsValidClaim(claim, out var message))
                return BadRequest(new { status = false, message });
            try
            {
                var identityClaim = _mapper.Map<Domain.Entities.IdentityClaim>(claim);
                _transactionService.ExecuteTransaction(() => { _authorizationService.AddIdentityClaim(identityClaim); });
                return Ok(new { status = true, message = "Add claim successfully" });
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while adding claim");
                return StatusCode(StatusCodes.Status500InternalServerError, new { status = false, message = "An error occurred while processing your request" });
            }
        }

        /// <summary>
        /// Cập nhật quyền của người dùng
        /// </summary>
        /// <param name="claim">Đối tượng quyền</param>
        /// <returns></returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT /api/v1/Authorization
        ///     {
        ///         "userId": "string",
        ///         "claimType": "string",
        ///         "claimValue": "string"
        ///     }
        /// </remarks>
        /// <response code="200">Cập nhật claim thành công</response>
        /// <response code="400">Lỗi dữ liệu đầu vào</response>
        /// <response code="401">Không có quyền truy cập</response>
        /// <response code="408">Quá thời gian yêu cầu</response>
        /// <response code="429">Request quá nhiều</response>
        /// <response code="500">Lỗi server</response>
        [HttpPut]
        [Authorize(Policy = "Admin")]
        public IActionResult UpdateClaim([FromBody] Models.UserClaim claim)
        {
            if (!IsValidClaim(claim, out var message))
                return BadRequest(new { status = false, message });
            try
            {
                var identityClaim = _mapper.Map<Domain.Entities.IdentityClaim>(claim);
                _transactionService.ExecuteTransaction(() =>
                {
                    _authorizationService.UpdateIdentityClaim(identityClaim);
                });
                return Ok(new { status = true, message = "Update claim successfully" });
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while updating claim");
                return StatusCode(StatusCodes.Status500InternalServerError, new { status = false, message = "An error occurred while processing your request" });
            }
        }

        /// <summary>
        /// Xoá quyền của người dùng
        /// </summary>
        /// <param name="claim">Đối tượng quyền</param>
        /// <returns></returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     DELETE /api/v1/Authorization
        ///     {
        ///         "userId": "string",
        ///         "claimType": "string",
        ///         "claimValue": "string"
        ///     }
        /// </remarks>
        /// <response code="200">Xoá claim thành công</response>
        /// <response code="400">Lỗi dữ liệu đầu vào</response>
        /// <response code="401">Không có quyền truy cập</response>
        /// <response code="408">Quá thời gian yêu cầu</response>
        /// <response code="429">Request quá nhiều</response>
        /// <response code="500">Lỗi server</response>
        [HttpDelete]
        [Authorize(Policy = "Admin")]
        public IActionResult DeleteClaim([FromBody] Models.UserClaim claim)
        {
            if (!IsValidClaim(claim, out var message))
                return BadRequest(new { status = false, message });
            try
            {
                _transactionService.ExecuteTransaction(() =>
                {
                    if (claim.userId == null || claim.claimType == null || claim.claimValue == null) return;
                    _authorizationService.DeleteIdentityClaim(claim.userId, claim.claimType,
                            claim.claimValue);
                });
                return Ok(new { status = true, message = "Delete claim successfully" });
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while deleting claim");
                return StatusCode(StatusCodes.Status500InternalServerError, new { status = false, message = "An error occurred while processing your request" });
            }
        }
    }
}
