using Application.Services;
using Application.Transaction;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Models;

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
    /// Quản lý chi nhánh
    /// </summary>
    [Authorize]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class BranchController : ControllerBase
    {
        private readonly ILogger<TeacherController> _logger;
        private readonly IMapper _mapper;
        private readonly BranchService _branchService;
        private readonly TransactionService _transactionService;

        /// <inheritdoc />
        public BranchController(
            BranchService branchService, 
            TransactionService transactionService,
            ILogger<TeacherController> logger)
        {
            _logger = logger;
            _mapper = new MapperConfiguration(cfg => cfg.CreateMap<Branch, Domain.Entities.Branch>()).CreateMapper();
            _branchService = branchService;
            _transactionService = transactionService;
        }

        private bool IsValidBranch(Branch branch, out string? message)
        {
            if (string.IsNullOrEmpty(branch.name))
            {
                message = "Name of branch is required";
                return false;
            }

            if (_branchService.GetBranchById(branch.id) != null)
            {
                message = "Id of branch is duplicate";
                return false;
            }

            message = null;
            return true;
        }

        /// <summary>
        /// Lấy danh sách chi nhánh
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/v1/Branch
        /// </remarks>
        /// <response code="200">Lấy danh sách chi nhánh thành công</response>
        /// <response code="204">Không có chi nhánh nào</response>
        /// <response code="429">Request quá nhiều</response>
        /// <response code="500">Lỗi server</response>
        [HttpGet]
        [AllowAnonymous]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any, VaryByQueryKeys = new[] { "branches" })]
        public ActionResult GetBranches()
        {
            try
            {
                var branches = _branchService.GetBranches().ToArray();
                return branches.Any()
                    ? Ok(new { status = true, message = "Get data successfully", data = branches })
                    : NoContent();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while getting branch");
                return StatusCode(StatusCodes.Status500InternalServerError, new { status = false, message = "An error occurred while processing your request" });
            }
        }

        /// <summary>
        /// Lấy chi nhánh theo tên
        /// </summary>
        /// <param name="name">Tên chi nhánh</param>
        /// <returns></returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/v1/Branch/name/string
        /// </remarks>
        /// <response code="200">Lấy chi nhánh thành công</response>
        /// <response code="204">Không có chi nhánh nào</response>
        /// <response code="429">Request quá nhiều</response>
        /// <response code="500">Lỗi server</response>
        [HttpGet("name/{name}")]
        [AllowAnonymous]
        public ActionResult GetBranchesByNames([FromRoute] string? name)
        {
            try
            {
                var branches =  _branchService.GetBranchesByNames(name).ToArray();
                return branches.Any()
                    ? Ok(new { status = true, message = "Get data successfully", data = branches })
                    : NoContent();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while getting branch by name");
                return StatusCode(StatusCodes.Status500InternalServerError, new { status = false, message = "An error occurred while processing your request" });
            }
        }

        /// <summary>
        /// Lấy chi nhánh theo mã
        /// </summary>
        /// <param name="id">Mã chi nhánh</param>
        /// <returns></returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/v1/Branch/string
        /// </remarks>
        /// <response code="200">Lấy chi nhánh thành công</response>
        /// <response code="404">Không tìm thấy chi nhánh</response>
        /// <response code="429">Request quá nhiều</response>
        /// <response code="500">Lỗi server</response>
        [HttpGet("{id}")]
        [AllowAnonymous]
        public ActionResult GetBranchById([FromRoute] string? id)
        {
            try
            {
                var branch = _branchService.GetBranchById(id);
                return branch != null
                    ? Ok(new { status = true, message = "Get data successfully", data = branch })
                    : NotFound(new { status = false, message = "No branch was found" });
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while getting branch by id");
                return StatusCode(StatusCodes.Status500InternalServerError, new { status = false, message = "An error occurred while processing your request" });
            }
        }

        /// <summary>
        /// Lấy chi nhánh theo khu vực
        /// </summary>
        /// <param name="zone">Khu vực</param>
        /// <returns></returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/v1/Branch/zone/string
        /// </remarks>
        /// <response code="200">Lấy chi nhánh thành công</response>
        /// <response code="204">Không có chi nhánh nào</response>
        /// <response code="429">Request quá nhiều</response>
        /// <response code="500">Lỗi server</response>
        [HttpGet("zone/{zone}")]
        [AllowAnonymous]
        public ActionResult GetBranchByZone([FromRoute] string? zone)
        {
            try
            {
                var branch = _branchService.GetBranchesByZone(zone).ToArray();
                return branch.Any()
                    ? Ok(new { status = true, message = "Get data successfully", data = branch })
                    : NoContent();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while getting branch by zone");
                return StatusCode(StatusCodes.Status500InternalServerError, new { status = false, message = "An error occurred while processing your request" });
            }
        }

        /// <summary>
        /// Thêm chi nhánh
        /// </summary>
        /// <param name="branch">Đối tượng chi nhánh</param>
        /// <returns></returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/v1/Branch
        ///     {
        ///         "id": "string",
        ///         "name": "string",
        ///         "address": "string",
        ///     }
        /// </remarks>
        /// <response code="200">Thêm chi nhánh thành công</response>
        /// <response code="400">Lỗi dữ liệu đầu vào</response>
        /// <response code="401">Không có quyền</response>
        /// <response code="429">Request quá nhiều</response>
        /// <response code="500">Lỗi server</response>
        [HttpPost]
        [Authorize]
        public ActionResult AddBranch([FromBody] Branch branch)
        {
            if(!IsValidBranch(branch, out var message))
                return BadRequest(new { status = false, message });

            try
            {
                var branchEntity = _mapper.Map<Domain.Entities.Branch>(branch);
                _transactionService.ExecuteTransaction(() => { _branchService.AddBranch(branchEntity); });
                return Ok(new { status = true, message = "Add branch successfully" });
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while adding branch");
                return StatusCode(StatusCodes.Status500InternalServerError, new { status = false, message = "An error occurred while processing your request" });
            }
        }

        /// <summary>
        /// Cập nhật chi nhánh
        /// </summary>
        /// <param name="branch">Đối tượng chi nhánh</param>
        /// <returns></returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT /api/v1/Branch
        ///     {
        ///         "id": "string",
        ///         "name": "string",
        ///         "address": "string",
        ///     }
        /// </remarks>
        /// <response code="200">Cập nhật chi nhánh thành công</response>
        /// <response code="400">Lỗi dữ liệu đầu vào</response>
        /// <response code="401">Không có quyền</response>
        /// <response code="429">Request quá nhiều</response>
        /// <response code="500">Lỗi server</response>
        [HttpPut]
        [Authorize]
        public ActionResult UpdateBranch([FromBody] Branch branch)
        {
            if (!IsValidBranch(branch, out var message))
                return BadRequest(new { status = false, message });

            try
            {
                var branchEntity = _mapper.Map<Domain.Entities.Branch>(branch);
                _transactionService.ExecuteTransaction(() => { _branchService.UpdateBranch(branchEntity); });
                return Ok(new { status = true, message = "Update branch successfully" });
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while updating branch");
                return StatusCode(StatusCodes.Status500InternalServerError, new { status = false, message = "An error occurred while processing your request" });
            }
        }

        /// <summary>
        /// Xóa chi nhánh
        /// </summary>
        /// <param name="id">Mã chi nhánh</param>
        /// <returns></returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     DELETE /api/v1/Branch/string
        /// </remarks>
        /// <response code="200">Xóa chi nhánh thành công</response>
        /// <response code="401">Không có quyền</response>
        /// <response code="404">Không tìm thấy chi nhánh</response>
        /// <response code="429">Request quá nhiều</response>
        /// <response code="500">Lỗi server</response>
        [HttpDelete("{id}")]
        public ActionResult DeleteBranch([FromRoute] string id)
        {
            try
            {
                if (_branchService.GetBranchById(id) == null)
                    return NotFound(new { status = false, message = "No branch was found" });
                _transactionService.ExecuteTransaction(() => { _branchService.DeleteBranch(id); });
                return Ok(new { status = true, message = "Delete branch successfully" });
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while deleting branch");
                return StatusCode(StatusCodes.Status500InternalServerError, new { status = false, message = "An error occurred while processing your request" });
            }
        }
    }
}
