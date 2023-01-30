using Application.Services;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
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
    public partial class TeacherController : ControllerBase
    {
        private readonly ILogger<TeacherController> _logger;
        private readonly TeacherService _teacherService;

        [GeneratedRegex("^([\\w\\.\\-]+)@([\\w\\-]+)((\\.(\\w){2,3})+)$")]
        private static partial Regex EmailPattern();
        [GeneratedRegex("^([0-9]{10})$")]
        private static partial Regex PhonePattern();

        /// <inheritdoc />
        public TeacherController(TeacherService teacherService, ILogger<TeacherController> logger)
        {
            _logger = logger;
            _teacherService = teacherService;
        }

        private bool ValidData(string email, string phone, string? customerId, out string? message)
        {
            if (customerId != null)
            {
                var customer = _teacherService.GetAllId().FirstOrDefault(c => c == customerId);
                if (customer == null)
                {
                    _logger.LogWarning(
                        "[{time}]-[{millisecond},{nanosecond}] - WARMING - TeacherController.cs:{pid} - [Transaction={tid}] - failed: {value} is not valid",
                        DateTime.Now, DateTime.Now.Millisecond, DateTime.Now.Nanosecond, Environment.ProcessId,
                        Activity.Current?.Id ?? HttpContext.TraceIdentifier, customerId);
                    message = "Customer id is not exists";
                    return false;
                }
            }

            if (!EmailPattern().IsMatch(email))
            {
                _logger.LogWarning(
                    "[{time}]-[{millisecond},{nanosecond}] - WARMING - TeacherController.cs:{pid} - [Transaction={tid}] - failed: {value} is not an email",
                    DateTime.Now, DateTime.Now.Millisecond, DateTime.Now.Nanosecond, Environment.ProcessId,
                    Activity.Current?.Id ?? HttpContext.TraceIdentifier, email);
                message = "Email syntax is not correct";
                return false;
            }

            if (PhonePattern().IsMatch(phone))
            {
                message = null;
                return true;
            }

            _logger.LogWarning(
                "[{time}]-[{millisecond},{nanosecond}] - WARMING - TeacherController.cs:{pid} - [Transaction={tid}] - failed: {value} is not a phone number",
                DateTime.Now, DateTime.Now.Millisecond, DateTime.Now.Nanosecond, Environment.ProcessId,
                Activity.Current?.Id ?? HttpContext.TraceIdentifier, phone);
            message = "Phone syntax is not correct";
            return false;
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
        /// <response code="500">Lỗi server</response>
        [HttpGet("getTeachers")]
        public async Task<IActionResult> GetTeachers()
        {
            _logger.LogInformation("[{time}]-[{millisecond},{nanosecond}] - INFORMATION - TeacherController.cs:{pid} - [Transaction={tid}] - info: get all teacher", DateTime.Now, DateTime.Now.Microsecond, DateTime.Now.Nanosecond, Environment.ProcessId, Activity.Current?.Id ?? HttpContext.TraceIdentifier);
            try
            {
                var teachers = await Task.Run(_teacherService.GetTeachers);
                if (teachers.Count != 0)
                    return Ok(new { status = true, message = "Get data successfully", data = teachers });
                _logger.LogWarning("[{time}]-[{millisecond},{nanosecond}] - WARMING - TeacherController.cs:{pid} - [Transaction={tid}] - failed: no teacher found in database", DateTime.Now, DateTime.Now.Millisecond, DateTime.Now.Nanosecond, Environment.ProcessId, Activity.Current?.Id ?? HttpContext.TraceIdentifier);
                return NoContent();
            }
            catch (Exception e)
            {
                _logger.LogError("[{time}]-[{millisecond},{nanosecond}] - ERROR - TeacherController.cs:{pid} - [Transaction={tid}] - error: {errorMessage}", DateTime.Now, DateTime.Now.Microsecond, DateTime.Now.Nanosecond, Environment.ProcessId, Activity.Current?.Id ?? HttpContext.TraceIdentifier, e.Message);
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
        ///     {
        ///         "name": "string"
        ///     }
        /// </remarks>
        /// <response code="200">Lấy danh sách giáo viên thành công</response>
        /// <response code="204">Không có giáo viên nào</response>
        /// <response code="500">Lỗi server</response>
        [HttpGet("findByName/{name}")]
        public async Task<IActionResult> GetTeachersByName([FromRoute] string name)
        {
            _logger.LogInformation("[{time}]-[{millisecond},{nanosecond}] - INFORMATION - TeacherController.cs:{pid} - [Transaction={tid}] - info: get teacher by name", DateTime.Now, DateTime.Now.Microsecond, DateTime.Now.Nanosecond, Environment.ProcessId, Activity.Current?.Id ?? HttpContext.TraceIdentifier);
            try
            {
                var teachers = await Task.Run(() => _teacherService.FindTeacherByName(name));
                if (teachers.Count != 0)
                    return Ok(new { status = true, message = "Get data successfully", data = teachers });
                _logger.LogWarning("[{time}]-[{millisecond},{nanosecond}] - WARMING - TeacherController.cs:{pid} - [Transaction={tid}] - failed: no teacher named {name} was found", DateTime.Now, DateTime.Now.Millisecond, DateTime.Now.Nanosecond, Environment.ProcessId, Activity.Current?.Id ?? HttpContext.TraceIdentifier, name);
                return NoContent();
            }
            catch (Exception e)
            {
                _logger.LogError("[{time}]-[{millisecond},{nanosecond}] - ERROR - TeacherController.cs:{pid} - [Transaction={tid}] - error: {errorMessage}", DateTime.Now, DateTime.Now.Microsecond, DateTime.Now.Nanosecond, Environment.ProcessId, Activity.Current?.Id ?? HttpContext.TraceIdentifier, e.Message);
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
        ///     {
        ///         "id": "uuid
        ///     }
        /// </remarks>
        /// <response code="200">Lấy giáo viên thành công</response>
        /// <response code="204">Không có giáo viên nào</response>
        /// <response code="500">Lỗi server</response>
        [HttpGet("getTeacherById/{id:guid}")]
        public async Task<IActionResult> GetTeacherById([FromRoute] Guid id)
        {
            _logger.LogInformation("[{time}]-[{millisecond},{nanosecond}] - INFORMATION - TeacherController.cs:{pid} - [Transaction={tid}] - info: get teacher by id", DateTime.Now, DateTime.Now.Microsecond, DateTime.Now.Nanosecond, Environment.ProcessId, Activity.Current?.Id ?? HttpContext.TraceIdentifier);
            try
            {
                var teacher = await Task.Run(() => _teacherService.GetTeacherById(id));
                if (teacher != null)
                    return Ok(new { status = true, message = "Get data successfully", data = teacher });
                _logger.LogWarning("[{time}]-[{millisecond},{nanosecond}] - WARMING - TeacherController.cs:{pid} - [Transaction={tid}] - failed: no teacher id {name} was found", DateTime.Now, DateTime.Now.Millisecond, DateTime.Now.Nanosecond, Environment.ProcessId, Activity.Current?.Id ?? HttpContext.TraceIdentifier, id);
                return NoContent();

            }
            catch (Exception e)
            {
                _logger.LogError("[{time}]-[{millisecond},{nanosecond}] - ERROR - TeacherController.cs:{pid} - [Transaction={tid}] - error: {errorMessage}", DateTime.Now, DateTime.Now.Microsecond, DateTime.Now.Nanosecond, Environment.ProcessId, Activity.Current?.Id ?? HttpContext.TraceIdentifier, e.Message);
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
        /// <response code="500">Lỗi server</response>
        [HttpPost("addTeacher")]
        public async Task<IActionResult> AddTeacher([FromBody] Model.Teacher teacher)
        {
            _logger.LogInformation("[{time}]-[{millisecond},{nanosecond}] - INFORMATION - TeacherController.cs:{pid} - [Transaction={tid}] - info: add teacher", DateTime.Now, DateTime.Now.Microsecond, DateTime.Now.Nanosecond, Environment.ProcessId, Activity.Current?.Id ?? HttpContext.TraceIdentifier);
            if (teacher.fullName == null && teacher.email == null && teacher.phone == null)
            {
                _logger.LogWarning("[{time}]-[{millisecond},{nanosecond}] - WARMING - TeacherController.cs:{pid} - [Transaction={tid}] - failed: full name, email and phone are null", DateTime.Now, DateTime.Now.Millisecond, DateTime.Now.Nanosecond, Environment.ProcessId, Activity.Current?.Id ?? HttpContext.TraceIdentifier);
                return BadRequest(new { status = false, message = "Full name, email and phone are required" });
            }

            Domain.Entities.Teacher newTeacher = new()
            {
                id = new Guid(),
                fullName = teacher.fullName,
                email = teacher.email,
                phone = teacher.phone,
                customerId = teacher.customerId
            };

            if (newTeacher.phone != null && newTeacher.email != null && !ValidData(newTeacher.email, newTeacher.phone, newTeacher.customerId, out var result))
                return BadRequest(new { status = false, message = result });

            try
            {
                _logger.LogDebug("[{time}]-[{millisecond},{nanosecond}] - DEBUG - TeacherController.cs:{pid} - [Transaction={tid}] - debug: add teacher {value}",
                    DateTime.Now, DateTime.Now.Millisecond, DateTime.Now.Nanosecond, Environment.ProcessId,
                    Activity.Current?.Id ?? HttpContext.TraceIdentifier, newTeacher);
                await _teacherService.AddTeacher(newTeacher);
                return Ok(new { status = true, message = "Add teacher successfully" });
            }
            catch (Exception e)
            {
                _logger.LogError("[{time}]-[{millisecond},{nanosecond}] - ERROR - TeacherController.cs:{pid} - [Transaction={tid}] - error: {errorMessage}", DateTime.Now, DateTime.Now.Microsecond, DateTime.Now.Nanosecond, Environment.ProcessId, Activity.Current?.Id ?? HttpContext.TraceIdentifier, e.Message);
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
        /// <response code="500">Lỗi server</response>
        [HttpPut("updateTeacher/{id:guid}")]
        public async Task<IActionResult> UpdateTeacher([FromBody] Model.Teacher teacher, [FromRoute] Guid id)
        {
            _logger.LogInformation("[{time}]-[{millisecond},{nanosecond}] - INFORMATION - TeacherController.cs:{pid} - [Transaction={tid}] - info: update teacher", DateTime.Now, DateTime.Now.Microsecond, DateTime.Now.Nanosecond, Environment.ProcessId, Activity.Current?.Id ?? HttpContext.TraceIdentifier);
            MapperConfiguration config = new(cfg => cfg.CreateMap<Model.Teacher, Domain.Entities.Teacher>());
            var mapper = config.CreateMapper();
            try
            {
                var existTeacher = await Task.Run(() => _teacherService.GetTeachers().FirstOrDefault(x => x.id == id));
                if (existTeacher == null)
                {
                    _logger.LogWarning(
                        "[{time}]-[{millisecond},{nanosecond}] - WARMING - TeacherController.cs:{pid} - [Transaction={tid}] - failed: teacher {value} is not exists",
                        DateTime.Now, DateTime.Now.Microsecond, DateTime.Now.Nanosecond, Environment.ProcessId,
                        Activity.Current?.Id ?? HttpContext.TraceIdentifier, id);
                    return NotFound(new { status = false, message = "Teacher not found" });
                }

                if (teacher.phone != null && teacher.email != null && !ValidData(teacher.email, teacher.phone, teacher.customerId, out var result))
                    return BadRequest(new { status = false, message = result });

                mapper.Map(teacher, existTeacher);
                await _teacherService.UpdateTeacher(existTeacher, id);
                return Ok(new { status = true, message = "Update teacher successfully" });
            }
            catch (Exception e)
            {
                _logger.LogError("[{time}]-[{millisecond},{nanosecond}] - ERROR - TeacherController.cs:{pid} - [Transaction={tid}] - error: {errorMessage}", DateTime.Now, DateTime.Now.Microsecond, DateTime.Now.Nanosecond, Environment.ProcessId, Activity.Current?.Id ?? HttpContext.TraceIdentifier, e.Message);
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
        ///     {
        ///         "id": "uuid"
        ///     }
        /// </remarks>
        /// <response code="200">Xóa giáo viên thành công</response>
        /// <response code="404">Không tìm thấy giáo viên</response>
        /// <response code="500">Lỗi server</response>
        [HttpDelete("deleteTeacher/{id:guid}")]
        public async Task<IActionResult> DeleteTeacher([FromRoute] Guid id)
        {
            _logger.LogInformation("[{time}]-[{millisecond},{nanosecond}] - INFORMATION - TeacherController.cs:{pid} - [Transaction={tid}] - info: delete teacher", DateTime.Now, DateTime.Now.Microsecond, DateTime.Now.Nanosecond, Environment.ProcessId, Activity.Current?.Id ?? HttpContext.TraceIdentifier);
            try
            {
                if (await Task.Run(() => _teacherService.GetTeachers().FirstOrDefault(x => x.id == id)) == null)
                {
                    _logger.LogWarning(
                        "[{time}]-[{millisecond},{nanosecond}] - WARMING - TeacherController.cs:{pid} - [Transaction={tid}] - failed: teacher {value} is not exists",
                        DateTime.Now, DateTime.Now.Microsecond, DateTime.Now.Nanosecond, Environment.ProcessId,
                        Activity.Current?.Id ?? HttpContext.TraceIdentifier, id);
                    return NotFound(new { status = false, message = "Teacher not found" });
                }
                await _teacherService.DeleteTeacher(id);
                return Ok(new { status = true, message = "Delete teacher successfully" });
            }
            catch (Exception e)
            {
                _logger.LogError("[{time}]-[{millisecond},{nanosecond}] - ERROR - TeacherController.cs:{pid} - [Transaction={tid}] - error: {errorMessage}", DateTime.Now, DateTime.Now.Microsecond, DateTime.Now.Nanosecond, Environment.ProcessId, Activity.Current?.Id ?? HttpContext.TraceIdentifier, e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new { status = false, message = "An error occurred while processing your request" });
            }
        }
    }
}
