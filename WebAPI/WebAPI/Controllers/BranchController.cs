using Application.Services;
using AutoMapper;
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
    [Route("api/v1/[controller]")]
    [ApiController]
    public class BranchController : ControllerBase
    {
        private readonly ILogger<TeacherController> _logger;
        private readonly IMapper _mapper;
        private readonly BranchService _branchService;

        /// <inheritdoc />
        public BranchController(BranchService branchService, ILogger<TeacherController> logger)
        {
            _logger = logger;
            _mapper = new MapperConfiguration(cfg => cfg.CreateMap<Branch, Domain.Entities.Branch>()).CreateMapper();
            _branchService = branchService;
        }

        private async Task<(bool, string?)> IsValidBranch(Branch branch)
        {
            if (string.IsNullOrEmpty(branch.name))
                return (false, "Name of branch is required");

            return await _branchService.GetBranchById(branch.id) != null ? (false, "Id of branch is duplicate") : (true, null);
        }

        /// <summary>
        /// Lấy danh sách chi nhánh
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/v1/branch/getBranches
        /// </remarks>
        /// <response code="200">Lấy danh sách chi nhánh thành công</response>
        /// <response code="204">Không có chi nhánh nào</response>
        /// <response code="429">Request quá nhiều</response>
        /// <response code="500">Lỗi server</response>
        [HttpGet("getBranches")]
        public async Task<IActionResult> GetBranches()
        {
            try
            {
                var branches = await Task.Run(_branchService.GetBranches);
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
        ///     GET /api/v1/branch/findByName/string
        /// </remarks>
        /// <response code="200">Lấy chi nhánh thành công</response>
        /// <response code="204">Không có chi nhánh nào</response>
        /// <response code="429">Request quá nhiều</response>
        /// <response code="500">Lỗi server</response>
        [HttpGet("findByName/{name}")]
        public async Task<IActionResult> GetBranchesByNames([FromRoute] string? name)
        {
            try
            {
                var branches = await Task.Run(() => _branchService.GetBranchesByNames(name));
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
        ///     GET /api/v1/branch/getById/string
        /// </remarks>
        /// <response code="200">Lấy chi nhánh thành công</response>
        /// <response code="404">Không tìm thấy chi nhánh</response>
        /// <response code="429">Request quá nhiều</response>
        /// <response code="500">Lỗi server</response>
        [HttpGet("findById/{id}")]
        public async Task<IActionResult> GetBranchById([FromRoute] string? id)
        {
            try
            {
                var branch = await Task.Run(() => _branchService.GetBranchById(id));
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
        ///     GET /api/v1/branch/findByZone/string
        /// </remarks>
        /// <response code="200">Lấy chi nhánh thành công</response>
        /// <response code="204">Không có chi nhánh nào</response>
        /// <response code="429">Request quá nhiều</response>
        /// <response code="500">Lỗi server</response>
        [HttpGet("findByZone/{zone}")]
        public async Task<IActionResult> GetBranchByZone([FromRoute] string? zone)
        {
            try
            {
                var branch = await Task.Run(() => _branchService.GetBranchesByZone(zone));
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
        ///     POST /api/v1/branch/addBranch
        ///     {
        ///         "id": "string",
        ///         "name": "string",
        ///         "address": "string",
        ///     }
        /// </remarks>
        /// <response code="200">Thêm chi nhánh thành công</response>
        /// <response code="400">Lỗi dữ liệu đầu vào</response>
        /// <response code="429">Request quá nhiều</response>
        /// <response code="500">Lỗi server</response>
        [HttpPost("addBranch")]
        public async Task<IActionResult> AddBranch([FromBody] Branch branch)
        {
            var (isValid, message) = await Task.Run(() => IsValidBranch(branch));
            if (!isValid)
                return BadRequest(new { status = false, message });

            try
            {
                var branchEntity = _mapper.Map<Domain.Entities.Branch>(branch);
                await Task.Run(() => _branchService.AddBranch(branchEntity));
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
        /// <param name="id">Mã chi nhánh</param>
        /// <returns></returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT /api/v1/branch/updateBranch/string
        ///     {
        ///         "id": "string",
        ///         "name": "string",
        ///         "address": "string",
        ///     }
        /// </remarks>
        /// <response code="200">Cập nhật chi nhánh thành công</response>
        /// <response code="400">Lỗi dữ liệu đầu vào</response>
        /// <response code="429">Request quá nhiều</response>
        /// <response code="500">Lỗi server</response>
        [HttpPut("updateBranch/{id}")]
        public async Task<IActionResult> UpdateBranch([FromBody] Branch branch, [FromRoute] string id)
        {
            var (isValid, message) = await Task.Run(() => IsValidBranch(branch));
            if (!isValid)
                return BadRequest(new { status = false, message });

            try
            {
                var branchEntity = _mapper.Map<Domain.Entities.Branch>(branch);
                await Task.Run(() => _branchService.UpdateBranch(branchEntity, id));
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
        ///     DELETE /api/v1/branch/deleteBranch/string
        /// </remarks>
        /// <response code="200">Xóa chi nhánh thành công</response>
        /// <response code="404">Không tìm thấy chi nhánh</response>
        /// <response code="429">Request quá nhiều</response>
        /// <response code="500">Lỗi server</response>
        [HttpDelete("deleteBranch/{id}")]
        public async Task<IActionResult> DeleteBranch([FromRoute] string id)
        {
            try
            {
                if (await Task.Run(() => _branchService.GetBranchById(id)) == null)
                    return NotFound(new { status = false, message = "No branch was found" });
                await Task.Run(() => _branchService.DeleteBranch(id));
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
