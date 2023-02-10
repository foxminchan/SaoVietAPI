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
    /// Quản lý giáo viên
    /// </summary>
    [Route("api/v1/[controller]")]
    [ApiController]
    public class TeacherController : ControllerBase
    {
        private readonly ILogger<TeacherController> _logger;
        private readonly IMapper _mapper;
        private readonly TeacherService _teacherService;

        /// <inheritdoc />
        public TeacherController(TeacherService teacherService, ILogger<TeacherController> logger)
        {
            _logger = logger;
            _mapper = new MapperConfiguration(cfg => cfg.CreateMap<Models.Teacher, Domain.Entities.Teacher>()).CreateMapper();
            _teacherService = teacherService;
        }

        private async Task<(bool, string?)> IsValidTeacher(Models.Teacher teacher)
        {
            if (string.IsNullOrWhiteSpace(teacher.fullName) &&
                string.IsNullOrWhiteSpace(teacher.email) &&
                string.IsNullOrWhiteSpace(teacher.phone))
                return (false, "Full name, email and phone are required");

            if (!string.IsNullOrWhiteSpace(teacher.email) &&
                !Regex.IsMatch(teacher.email, @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", RegexOptions.None, TimeSpan.FromSeconds(2)))
                return (false, "Email is invalid");

            if (!string.IsNullOrWhiteSpace(teacher.phone) &&
                    !Regex.IsMatch(teacher.phone, @"^([0-9]{10})$", RegexOptions.None, TimeSpan.FromSeconds(2)))
                return (false, "Phone is invalid");

            var allId = await _teacherService.GetAllId();
            if (string.IsNullOrWhiteSpace(teacher.customerId)
                || allId.Contains(teacher.customerId)) return (true, null);
            return (false, "Customer id is not exists");

        }

        /// <summary>
        /// Lấy danh sách giáo viên
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /getTeachers
        /// </remarks>
        /// <response code="200">Lấy danh sách giáo viên thành công</response>
        /// <response code="204">Không có giáo viên nào</response>
        /// <response code="429">Request quá nhiều</response>
        /// <response code="500">Lỗi server</response>
        [HttpGet("getTeachers")]
        public async Task<IActionResult> GetTeachers()
        {
            try
            {
                var teachers = await Task.Run(_teacherService.GetTeachers);
                return teachers.Any()
                    ? Ok(new { status = true, message = "Get data successfully", data = teachers })
                    : NoContent();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while getting all teacher");
                return StatusCode(StatusCodes.Status500InternalServerError, new { status = false, message = "An error occurred while processing your request" });
            }
        }

        /// <summary>
        /// Lấy giáo viên theo tên
        /// </summary>
        /// <param name="name">Tên giáo viên</param>
        /// <returns></returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/v1/teacher/findByName/string
        /// </remarks>
        /// <response code="200">Lấy danh sách giáo viên thành công</response>
        /// <response code="204">Không có giáo viên nào</response>
        /// <response code="429">Request quá nhiều</response>
        /// <response code="500">Lỗi server</response>
        [HttpGet("findByName/{name}")]
        public async Task<IActionResult> GetTeachersByName([FromRoute] string name)
        {
            try
            {
                var teachers = await Task.Run(() => _teacherService.FindTeacherByName(name));
                return teachers.Any()
                    ? Ok(new { status = true, message = "Get data successfully", data = teachers })
                    : NoContent();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while getting teacher by name");
                return StatusCode(StatusCodes.Status500InternalServerError, new { status = false, message = "An error occurred while processing your request" });
            }
        }

        /// <summary>
        /// Tìm kiếm giáo viên theo mã
        /// </summary>
        /// <param name="id">Mã giáo viên</param>
        /// <returns></returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/v1/teacher/uuid
        /// </remarks>
        /// <response code="200">Lấy giáo viên thành công</response>
        /// <response code="204">Không có giáo viên nào</response>
        /// <response code="429">Request quá nhiều</response>
        /// <response code="500">Lỗi server</response>
        [HttpGet("findById/{id:guid}")]
        public async Task<IActionResult> GetTeacherById([FromRoute] Guid id)
        {
            try
            {
                var teacher = await Task.Run(() => _teacherService.GetTeacherById(id));
                return teacher != null
                    ? Ok(new { status = true, message = "Get data successfully", data = teacher })
                    : NoContent();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while getting teacher by id");
                return StatusCode(StatusCodes.Status500InternalServerError, new { status = false, message = "An error occurred while processing your request" });
            }
        }

        /// <summary>
        /// Thêm giáo viên
        /// </summary>
        /// <param name="teacher">Đối tượng giáo viên</param>
        /// <returns></returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /addTeacher
        ///     {
        ///         "fullName": "string",
        ///         "email": "string",
        ///         "phone": "string",
        ///         "customerId": "uuid"
        ///     }
        /// </remarks>
        /// <response code="200">Thêm giáo viên thành công</response>
        /// <response code="400">Lỗi dữ liệu đầu vào</response>
        /// <response code="429">Request quá nhiều</response>
        /// <response code="500">Lỗi server</response>
        [HttpPost("addTeacher")]
        public async Task<IActionResult> AddTeacher([FromBody] Models.Teacher teacher)
        {
            var (isValid, message) = await Task.Run(() => IsValidTeacher(teacher));
            if (!isValid)
                return BadRequest(new { status = false, message });

            try
            {
                var newTeacher = _mapper.Map<Domain.Entities.Teacher>(teacher);
                newTeacher.id = Guid.NewGuid();
                await Task.Run(() => _teacherService.AddTeacher(newTeacher));
                return Ok(new { status = true, message = "Add teacher successfully" });
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while adding teacher");
                return StatusCode(StatusCodes.Status500InternalServerError, new { status = false, message = "An error occurred while processing your request" });
            }
        }

        /// <summary>
        /// Cập nhật giáo viên
        /// </summary>
        /// <param name="teacher">Đối tượng giáo viên</param>
        /// <param name="id">Mã giáo viên</param>
        /// <returns></returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT /updateTeacher/uuid
        ///     {
        ///         "id": "uuid",
        ///         "fullName": "string",
        ///         "email": "string",
        ///         "phone": "string",
        ///         "customerId": "string"
        ///     }
        /// </remarks>
        /// <response code="200">Cập nhật giáo viên thành công</response>
        /// <response code="404">Không tìm thấy giáo viên</response>
        /// <response code="429">Request quá nhiều</response>
        /// <response code="500">Lỗi server</response>
        [HttpPut("updateTeacher/{id:guid}")]
        public async Task<IActionResult> UpdateTeacher([FromBody] Models.Teacher teacher, [FromRoute] Guid id)
        {
            try
            {
                var existTeacher = await Task.Run(() => _teacherService.GetTeacherById(id));
                if (existTeacher == null)
                    return NotFound(new { status = false, message = "Teacher not found" });
                var (isValid, message) = await Task.Run(() => IsValidTeacher(teacher));
                if (!isValid)
                    return BadRequest(new { status = false, message });
                var updatedTeacher = _mapper.Map(teacher, existTeacher);
                await Task.Run(() => _teacherService.UpdateTeacher(updatedTeacher, id));
                return Ok(new { status = true, message = "Update teacher successfully" });
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while updating teacher");
                return StatusCode(StatusCodes.Status500InternalServerError, new { status = false, message = "An error occurred while processing your request" });
            }
        }

        /// <summary>
        /// Xóa giáo viên
        /// </summary>
        /// <param name="id">Mã giáo viên</param>
        /// <returns></returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     DELETE /deleteTeacher/uuid
        /// </remarks>
        /// <response code="200">Xóa giáo viên thành công</response>
        /// <response code="404">Không tìm thấy giáo viên</response>
        /// <response code="429">Request quá nhiều</response>
        /// <response code="500">Lỗi server</response>
        [HttpDelete("deleteTeacher/{id:guid}")]
        public async Task<IActionResult> DeleteTeacher([FromRoute] Guid id)
        {
            try
            {
                var teacher = await Task.Run(() => _teacherService.GetTeacherById(id));
                if (teacher == null)
                    return NotFound(new { status = false, message = "Teacher not found" });
                await Task.Run(() => _teacherService.DeleteTeacher(id));
                return Ok(new { status = true, message = "Delete teacher successfully" });
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while deleting teacher");
                return StatusCode(StatusCodes.Status500InternalServerError, new { status = false, message = "An error occurred while processing your request" });
            }
        }
    }
}