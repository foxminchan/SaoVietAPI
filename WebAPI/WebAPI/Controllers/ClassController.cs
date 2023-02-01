using Application.Services;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
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
    /// Quản lý lớp học
    /// </summary>
    [Route("api/v1/[controller]")]
    [ApiController]
    public partial class ClassController : ControllerBase
    {
        private readonly ILogger<TeacherController> _logger;
        private readonly IMapper _mapper;
        private readonly ClassService _classService;

        [GeneratedRegex("^\\d{4}-\\d{2}-\\d{2}$")]
        private static partial Regex DateFormatRegex();

        /// <inheritdoc />
        public ClassController(ClassService classService, ILogger<TeacherController> logger)
        {
            _logger = logger;
            _mapper = new MapperConfiguration(cfg => cfg.CreateMap<Model.Class, Domain.Entities.Class>()).CreateMapper();
            _classService = classService;
        }

        private bool ValidData(Model.Class @class, out string? message)
        {
            message = string.Empty;
            if (string.IsNullOrWhiteSpace(@class.name))
            {
                message = "Class name is required";
                return false;
            }

            if (!string.IsNullOrWhiteSpace(@class.startDate) &&
                !DateFormatRegex().IsMatch(@class.startDate))
            {
                message = "Start date must be match YYYY-MM-DD format";
                return false;
            }

            if (!string.IsNullOrWhiteSpace(@class.endDate) &&
                !DateFormatRegex().IsMatch(@class.endDate))
            {
                message = "End date must be match YYYY-MM-DD format";
                return false;
            }

            if (!string.IsNullOrWhiteSpace(@class.startDate) &&
                !string.IsNullOrWhiteSpace(@class.endDate) &&
                DateTime.Parse(@class.startDate) > DateTime.Parse(@class.endDate))
            {
                message = "Start date must be less than end date";
                return false;
            }

            if (string.IsNullOrWhiteSpace(@class.id)
                || _classService.GetClasses().Any(x => x.id == @class.id))
            {
                message = "Class id is exist";
                return false;
            }

            if (@class.teacherId != null
                && !_classService.GetTeachers().Any(x => x != null && x.id == @class.teacherId))
            {
                message = "Teacher id is not exist";
                return false;
            }

            if (@class.branchId == null
                || _classService.GetBranches().Any(x => x != null && x.id == @class.branchId)) return true;
            message = "Branch id is not exist";
            return false;

        }

        /// <summary>
        /// Lấy danh sách lớp học
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /getClasses
        /// </remarks>
        /// <response code="200">Lấy danh sách lớp học thành công</response>
        /// <response code="204">Không có lớp học nào</response>
        /// <response code="500">Lỗi server</response>
        [HttpGet("getClasses")]
        public async Task<IActionResult> GetClasses()
        {
            try
            {
                var classes = await Task.Run(_classService.GetClasses);
                return classes.Any()
                    ? Ok(new { status = true, message = "Get data successfully", data = classes })
                    : NoContent();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while getting all class");
                return StatusCode(StatusCodes.Status500InternalServerError, new { status = false, message = "An error occurred while processing your request" });
            }
        }

        /// <summary>
        /// Tìm lớp theo tên
        /// </summary>
        /// <param name="name">Tên lớp</param>
        /// <returns></returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/v1/class/findByName/string
        ///     {
        ///         "name": "string"
        ///     }
        /// </remarks>
        /// <response code="200">Tìm lớp theo tên thành công</response>
        /// <response code="400">Lỗi dữ liệu đầu vào</response>
        /// <response code="404">Không tìm thấy lớp</response>
        /// <response code="500">Lỗi server</response>
        [HttpGet("findByName/{name}")]
        public async Task<IActionResult> FindClassByName([FromRoute] string? name)
        {
            if (name == null)
                return BadRequest(new { status = false, message = "Name is null" });
            try
            {
                var classEntity = await Task.Run(() => _classService.FindClassByName(name));
                return !classEntity.Any()
                    ? NotFound(new { status = false, message = "Class is not found" })
                    : Ok(new { status = true, message = "Find class successfully", data = classEntity });
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while finding class by name");
                return StatusCode(StatusCodes.Status500InternalServerError, new { status = false, message = "An error occurred while processing your request" });
            }
        }

        /// <summary>
        /// Tìm lớp theo trạng thái
        /// </summary>
        /// <param name="status">Trạng thái</param>
        /// <returns></returns>
        /// <remarks>
        /// Status:
        /// - ```Expired```: Lớp đã kết thúc
        /// - ```Active```: Lớp đang diễn ra
        /// - ```Upcoming```: Lớp sắp diễn ra
        /// 
        /// Sample request:
        ///
        ///     GET /api/v1/class/findByStatus/string
        ///     {
        ///         "status": "string"
        ///     }
        /// </remarks>
        /// <response code="200">Tìm lớp theo trạng thái thành công</response>
        /// <response code="400">Lỗi dữ liệu đầu vào</response>
        /// <response code="204">Không tìm thấy lớp</response>
        /// <response code="500">Lỗi server</response>
        [HttpGet("findByStatus/{status:regex(Expired|Active|Upcoming)}")]
        public async Task<IActionResult> FindClassByStatus([FromRoute] string? status)
        {
            try
            {
                var classEntity = await Task.Run(() => _classService.GetClassesByStatus(status));
                return !classEntity.Any()
                    ? NoContent()
                    : Ok(new { status = true, message = "Find class successfully", data = classEntity });
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while finding class by status");
                return StatusCode(StatusCodes.Status500InternalServerError, new { status = false, message = "An error occurred while processing your request" });
            }
        }

        /// <summary>
        /// Liệt kê lớp theo giáo viên
        /// </summary>
        /// <param name="teacherId">Mã giáo viên</param>
        /// <returns></returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/v1/class/getClassesByTeacherId/Guid
        ///     {
        ///         "teacherId": "Guid"
        ///     }
        /// </remarks>
        /// <response code="200">Liệt kê lớp theo giáo viên thành công</response>
        /// <response code="400">Lỗi dữ liệu đầu vào</response>
        /// <response code="404">Không tìm thấy lớp</response>
        /// <response code="500">Lỗi server</response>
        [HttpGet("findByTeacher/{teacherId:Guid}")]
        public async Task<IActionResult> GetClassesByTeacherId([FromRoute] Guid? teacherId)
        {
            if (teacherId == null)
                return BadRequest(new { status = false, message = "TeacherId is null" });
            
            try
            {
                var classEntity = await Task.Run(() => _classService.FindClassByTeacher(teacherId));
                return !classEntity.Any()
                    ? NotFound(new { status = false, message = "Class is not found" })
                    : Ok(new { status = true, message = "Find class successfully", data = classEntity });
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while finding class by teacherId");
                return StatusCode(StatusCodes.Status500InternalServerError, new { status = false, message = "An error occurred while processing your request" });
            }
        }

        /// <summary>
        /// Thêm lớp học
        /// </summary>
        /// <param name="request">Đối tượng lớp học</param>
        /// <returns></returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/v1/class/getClasses
        ///     {
        ///         "id": "string",
        ///         "name": "string",
        ///         "startDate": "yyyy-MM-dd",
        ///         "endDate": "yyyy-MM-dd",
        ///         "teacherId": "uuid",
        ///         "branchId": "string"
        ///     }
        /// </remarks>
        /// <response code="200">Thêm lớp học thành công</response>
        /// <response code="400">Lỗi dữ liệu đầu vào</response>
        /// <response code="500">Lỗi server</response>
        [HttpPost("addClass")]
        public async Task<IActionResult> AddClass([FromBody] Model.Class request)
        {
            if (!ValidData(request, out var message))
                return BadRequest(new { status = false, message });

            try
            {
                var newClass = _mapper.Map<Domain.Entities.Class>(request);
                await Task.Run(() => _classService.AddClass(newClass));
                return Ok(new { status = true, message = "Add class successfully" });
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while adding class");
                return StatusCode(StatusCodes.Status500InternalServerError, new { status = false, message = "An error occurred while processing your request" });
            }
        }

        /// <summary>
        /// Cập nhật thông tin lớp học
        /// </summary>
        /// <param name="request">Đối tượng lớp học</param>
        /// <param name="id">Mã lớp</param>
        /// <returns></returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT /api/v1/class/updateClass/string
        ///     {
        ///         "id": "string",
        ///         "name": "string",
        ///         "startDate": "yyyy-MM-dd",
        ///         "endDate": "yyyy-MM-dd",
        ///         "teacherId": "uuid",
        ///         "branchId": "string"
        ///     }
        /// </remarks>
        /// <response code="200">Cập nhật thông tin lớp học thành công</response>
        /// <response code="400">Lỗi dữ liệu đầu vào</response>
        /// <response code="500">Lỗi server</response>
        [HttpPut("updateClass/{id}")]
        public async Task<IActionResult> UpdateClass([FromBody] Model.Class request, [FromRoute] string id)
        {
            if (!ValidData(request, out var message))
                return BadRequest(new { status = false, message });

            try
            {
                var classEntity = _mapper.Map<Domain.Entities.Class>(request);
                await Task.Run(() => _classService.UpdateClass(classEntity, id));
                return Ok(new { status = true, message = "Update class successfully", data = classEntity });
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while updating class");
                return StatusCode(StatusCodes.Status500InternalServerError, new { status = false, message = "An error occurred while processing your request" });
            }
        }

        /// <summary>
        /// Xóa lớp học
        /// </summary>
        /// <param name="id">Mã lớp học</param>
        /// <returns></returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     DELETE /api/v1/class/deleteClass/string
        ///     {
        ///         "id": "string"
        ///     }
        /// </remarks>
        /// <response code="200">Xóa lớp học thành công</response>
        /// <response code="400">Lỗi dữ liệu đầu vào</response>
        /// <response code="500">Lỗi server</response>
        [HttpDelete("deleteClass/{id}")]
        public async Task<IActionResult> DeleteClass([FromRoute] string id)
        {
            try
            {
                if (await Task.Run(() => _classService.GetClasses().Any(x => x.id == id)) == false)
                    return BadRequest(new { status = false, message = "Class id is not exist" });
                await Task.Run(() => _classService.DeleteClass(id));
                return Ok(new { status = true, message = "Delete class successfully" });
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while deleting class");
                return StatusCode(StatusCodes.Status500InternalServerError, new { status = false, message = "An error occurred while processing your request" });
            }
        }
    }
}