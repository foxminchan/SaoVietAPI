﻿using Application.Services;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
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
            _mapper = new MapperConfiguration(cfg => cfg.CreateMap<Model.Branch, Domain.Entities.Branch>()).CreateMapper();
            _branchService = branchService;
        }

        private bool ValidData(Model.Branch branch, out string message)
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

            message = string.Empty;
            return true;
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
                return StatusCode(StatusCodes.Status500InternalServerError);
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
        ///     {
        ///         "name": "string"
        ///     }
        /// </remarks>
        /// <response code="200">Lấy chi nhánh thành công</response>
        /// <response code="204">Không có chi nhánh nào</response>
        /// <response code="500">Lỗi server</response>
        [HttpGet("findByName/{name}")]
        public async Task<IActionResult> GetBranchesByNames(string? name)
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
                return StatusCode(StatusCodes.Status500InternalServerError);
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
        ///     {
        ///         "id": "string"
        ///     }
        /// </remarks>
        /// <response code="200">Lấy chi nhánh thành công</response>
        /// <response code="404">Không tìm thấy chi nhánh</response>
        /// <response code="500">Lỗi server</response>
        [HttpGet("findById/{id}")]
        public async Task<IActionResult> GetBranchById(string? id)
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
        /// <response code="500">Lỗi server</response>
        [HttpPost("addBranch")]
        public async Task<IActionResult> AddBranch([FromBody] Model.Branch branch)
        {
            if (!ValidData(branch, out var message))
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
        /// <response code="500">Lỗi server</response>
        [HttpPut("updateBranch/{id}")]
        public async Task<IActionResult> UpdateBranch([FromBody] Model.Branch branch, [FromRoute] string id)
        {
            if (!ValidData(branch, out var message))
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
        ///     {
        ///         "id": "string"
        ///     }
        /// </remarks>
        /// <response code="200">Xóa chi nhánh thành công</response>
        /// <response code="404">Không tìm thấy chi nhánh</response>
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
