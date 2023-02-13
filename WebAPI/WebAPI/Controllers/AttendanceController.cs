using Application.Message;
using Application.Services;
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
    /// Quản lý điểm danh
    /// </summary>
    [Authorize]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AttendanceController : ControllerBase
    {
        private readonly ILogger<TeacherController> _logger;
        private readonly IMapper _mapper;
        private readonly AttendanceService _attendanceService;
        private readonly IRabbitMqService _rabbitMqService;

        /// <inheritdoc />
        public AttendanceController(AttendanceService attendanceService, ILogger<TeacherController> logger)
        {
            _logger = logger;
            _rabbitMqService = new RabbitMqService("attendanceQueue");
            _mapper = new MapperConfiguration(cfg => cfg.CreateMap<Models.Attendance, Domain.Entities.Attendance>()).CreateMapper();
            _attendanceService = attendanceService;
        }

        private async Task<(bool, string?)> IsValidAttendance(Models.Attendance attendance)
        {
            if(string.IsNullOrEmpty(attendance.classId) &&
               string.IsNullOrEmpty(attendance.lessonId))
                return (false, "ClassId and LessonId cannot be null or empty");
            if (attendance.evaluation is < 0 or > 10)
                return (false, "Evaluation must be between 0 and 10");
            if (attendance.attendance < 0)
                return (false, "Attendance must be greater than 0");
            if (attendance.lessonId != null && !await _attendanceService.CheckLessonExists(attendance.lessonId))
                return (false, "LessonId does not exist");
            if (attendance.classId != null && !await _attendanceService.CheckClassExists(attendance.classId))
                return (false, "ClassId does not exist");
            return (true, null);
        }

        /// <summary>
        /// Lấy danh sách sổ điểm danh
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/v1/Attendance
        /// </remarks>
        /// <response code="200">Lấy danh sách sổ điểm danh thành công</response>
        /// <response code="204">Không có sổ điểm danh nào</response>
        /// <response code="429">Request quá nhiều</response>
        /// <response code="500">Lỗi server</response>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAttendance()
        {
            try
            {
                var branches = await Task.Run(_attendanceService.GetAllAttendance);
                return branches.Any()
                    ? Ok(new { status = true, message = "Get data successfully", data = branches })
                    : NoContent();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while getting attendance");
                return StatusCode(StatusCodes.Status500InternalServerError, new { status = false, message = "An error occurred while processing your request" });
            }
        }

        /// <summary>
        /// Lấy danh sách sổ điểm danh theo lớp và bài học
        /// </summary>
        /// <param name="classId">Mã lớp</param>
        /// <param name="lessonId">Mã bài học</param>
        /// <returns></returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/v1/Attendance/string/string
        /// </remarks>
        /// <response code="200">Lấy danh sách sổ điểm danh thành công</response>
        /// <response code="204">Không có sổ điểm danh nào</response>
        /// <response code="429">Request quá nhiều</response>
        /// <response code="500">Lỗi server</response>
        [HttpGet("{classId}/{lessonId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAttendanceById([FromRoute] string classId, [FromRoute] string lessonId)
        {
            try
            {
                var attendance = await Task.Run(() => _attendanceService.GetAttendanceById(classId, lessonId));
                return attendance.Any()
                    ? Ok(new { status = true, message = "Get data successfully", data = attendance })
                    : NoContent();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while getting attendance");
                return StatusCode(StatusCodes.Status500InternalServerError, new { status = false, message = "An error occurred while processing your request" });
            }
        }

        /// <summary>
        /// Lấy danh sách sổ điểm danh theo lớp
        /// </summary>
        /// <param name="classId">Mã lớp</param>
        /// <returns></returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/v1/Attendance/string
        /// </remarks>
        /// <response code="200">Lấy danh sách sổ điểm danh thành công</response>
        /// <response code="204">Không có sổ điểm danh nào</response>
        /// <response code="429">Request quá nhiều</response>
        /// <response code="500">Lỗi server</response>
        [HttpGet("{classId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAttendanceByClassId([FromRoute] string classId)
        {
            try
            {
                var attendance = await Task.Run(() => _attendanceService.GetAttendanceByClassId(classId));
                return attendance.Any()
                    ? Ok(new { status = true, message = "Get data successfully", data = attendance })
                    : NoContent();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while getting attendance");
                return StatusCode(StatusCodes.Status500InternalServerError, new { status = false, message = "An error occurred while processing your request" });
            }
        }

        /// <summary>
        /// Sắp xếp sổ điểm danh
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/v1/Attendance/sort
        /// </remarks>
        /// <response code="200">Lấy danh sách sổ điểm danh thành công</response>
        /// <response code="204">Không có sổ điểm danh nào</response>
        /// <response code="429">Request quá nhiều</response>
        /// <response code="500">Lỗi server</response>
        [HttpGet("sort")]
        [AllowAnonymous]
        public async Task<IActionResult> SortByAttendance()
        {
            try
            {
                var attendance = await Task.Run(_attendanceService.SortByAttendance);
                return attendance.Any()
                    ? Ok(new { status = true, message = "Get data successfully", data = attendance })
                    : NoContent();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while getting attendance");
                return StatusCode(StatusCodes.Status500InternalServerError, new { status = false, message = "An error occurred while processing your request" });
            }
        }

        /// <summary>
        /// Thêm sổ điểm danh
        /// </summary>
        /// <param name="attendance">Đối tượng sổ điểm danh</param>
        /// <returns></returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/v1/Attendance
        ///     {
        ///         "classId": "string",
        ///         "lessonId": "string"
        ///         "date": "yyyy-MM-dd",
        ///         "comment": "string",
        ///         "evaluation": "tinyint",
        ///         "attendance": "tinyint"
        ///     }
        /// </remarks>
        /// <response code="200">Thêm sổ điểm danh thành công</response>
        /// <response code="400">Lỗi dữ liệu đầu vào</response>
        /// <response code="401">Không có quyền</response>
        /// <response code="429">Request quá nhiều</response>
        /// <response code="500">Lỗi server</response>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddAttendance([FromBody] Models.Attendance attendance)
        {
            var (isValid, message) = await Task.Run(() => IsValidAttendance(attendance));
            if (!isValid)
                return BadRequest(new { status = false, message });

            try
            {
                var attendanceEntity = _mapper.Map<Domain.Entities.Attendance>(attendance);
                await Task.Run(() => _attendanceService.AddAttendance(attendanceEntity));
                _rabbitMqService.SendMessage(attendance);
                return Ok(new { status = true, message = "Add attendance successfully" });
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while adding attendance");
                return StatusCode(StatusCodes.Status500InternalServerError, new { status = false, message = "An error occurred while processing your request" });
            }
        }

        /// <summary>
        /// Cập nhật sổ điểm danh
        /// </summary>
        /// <param name="attendance">Đối tượng sổ điểm danh</param>
        /// <param name="classId">Mã lớp</param>
        /// <param name="lessonId">Mã bài học</param>
        /// <returns></returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT /api/v1/Attendance/string/string
        ///     {
        ///         "classId": "string",
        ///         "lessonId": "string"
        ///         "date": "yyyy-MM-dd",
        ///         "comment": "string",
        ///         "evaluation": "tinyint",
        ///         "attendance": "tinyint"
        ///     }
        /// </remarks>
        /// <response code="200">Cập nhật sổ điểm danh thành công</response>
        /// <response code="400">Lỗi dữ liệu đầu vào</response>
        /// <response code="401">Không có quyền</response>
        /// <response code="429">Request quá nhiều</response>
        /// <response code="500">Lỗi server</response>
        [HttpPut("{classId}/{lessonId}")]
        [Authorize]
        public async Task<IActionResult> UpdateAttendance([FromBody] Models.Attendance attendance, [FromRoute] string classId, [FromRoute] string lessonId)
        {
            var (isValid, message) = await Task.Run(() => IsValidAttendance(attendance));
            if (!isValid)
                return BadRequest(new { status = false, message });

            try
            {
                var attendanceEntity = _mapper.Map<Domain.Entities.Attendance>(attendance);
                await Task.Run(() => _attendanceService.UpdateAttendance(attendanceEntity, classId, lessonId));
                return Ok(new { status = true, message = "Update attendance successfully" });
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while updating attendance");
                return StatusCode(StatusCodes.Status500InternalServerError, new { status = false, message = "An error occurred while processing your request" });
            }
        }

        /// <summary>
        /// Xoá sổ điểm danh
        /// </summary>
        /// <param name="classId">Mã lớp học</param>
        /// <param name="lessonId">Mã bài học</param>
        /// <returns></returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     DELETE /api/v1/Attendance/string/string
        /// </remarks>
        /// <response code="200">Xóa sổ điểm danh thành công</response>
        /// <response code="401">Không có quyền</response>
        /// <response code="404">Không tìm thấy sổ điểm danh</response>
        /// <response code="429">Request quá nhiều</response>
        /// <response code="500">Lỗi server</response>
        [HttpDelete("{classId}/{lessonId}")]
        [Authorize]
        public async Task<IActionResult> DeleteAttendance([FromRoute] string classId, [FromRoute] string lessonId)
        {
            if(!await Task.Run(() => _attendanceService.IsAttendanceExist(classId, lessonId)))
                return NotFound(new { status = false, message = "Attendance not found" });

            try
            {
                await Task.Run(() => _attendanceService.DeleteAttendance(classId, lessonId));
                return Ok(new { status = true, message = "Delete attendance successfully" });
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while deleting attendance");
                return StatusCode(StatusCodes.Status500InternalServerError, new { status = false, message = "An error occurred while processing your request" });
            }
        }
    }
}
