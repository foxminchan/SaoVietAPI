using System.Text.RegularExpressions;
using Application.Services;
using AutoMapper;
using Domain.Entities;
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

    [Route("api/v1/[controller]")]
    [ApiController]
    public class TeacherController : ControllerBase
    {
        private readonly ILogger<TeacherController> _logger;
        private readonly TeacherService _teacherService;

        /// <inheritdoc />
        public TeacherController(TeacherService teacherService, ILogger<TeacherController> logger)
        {
            _logger = logger;
            _teacherService = teacherService;
        }

        /// <summary>
        /// Lấy danh sách giáo viên
        /// </summary>
        /// <returns>Danh sách giáo viên</returns>
        /// <response code="200">Lấy danh sách giáo viên thành công</response>
        /// <response code="204">Không có giáo viên nào</response>
        /// <response code="500">Lỗi server</response>
        [HttpGet("getTeachers")]
        public async Task<IActionResult> GetTeachers()
        {
            try
            {
                _logger.LogInformation("Get teachers");
                var teachers = await Task.Run(() => _teacherService.GetTeachers());
                if (teachers.Count == 0)
                    return NoContent();
                return Ok(new { status = true, message = "Get data successfully", data = teachers });
            }
            catch (Exception)
            {
                _logger.LogError("GetTeachers");
                return StatusCode(StatusCodes.Status500InternalServerError, new { status = false, message = "An error occurred while processing your request" });
            }
        }

        /// <summary>
        /// Thêm giáo viên
        /// </summary>
        /// <param name="teacher">Đối tượng giáo viên</param>
        /// <returns>Giáo viên mới được tạo</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /addTeacher
        ///     {
        ///         "fullName": "Nguyễn Xuân Nhân",
        ///         "email": "nguyenxuannhan407@gmail.com",
        ///         "phone": "0123456789",
        ///         "customerId": null
        ///     }
        /// </remarks>
        /// <response code="200">Thêm giáo viên thành công</response>
        /// <response code="400">Lỗi dữ liệu đầu vào</response>
        /// <response code="500">Lỗi server</response>
        [HttpPost("addTeacher")]
        public async Task<IActionResult> AddTeacher([FromBody] Model.Teacher teacher)
        {
            Teacher newTeacher = new()
            {
                id = new Guid(),
                fullName = teacher.fullName,
                email = teacher.email,
                phone = teacher.phone,
                customerId = teacher.customerId
            };
            
            if (newTeacher.email != null && !Regex.IsMatch(newTeacher.email, @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$"))
                return BadRequest(new { status = false, message = "Email syntax is not correct" });
            if (newTeacher.phone != null && !Regex.IsMatch(newTeacher.phone, @"^([0-9]{10})$"))
                return BadRequest(new { status = false, message = "Phone syntax is not correct" });
            if (newTeacher.customerId != null)
            {
                var customer = await Task.Run(() => _teacherService.GetAllId().FirstOrDefault(c => c == newTeacher.customerId));
                if (customer == null)
                    return BadRequest(new { status = false, message = "Customer id is not exists" });
            }

            try
            {
                await _teacherService.AddTeacher(newTeacher);
                return Ok(new { status = true, message = "Add teacher successfully" });
            }
            catch (Exception)
            {
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
        ///     PUT /updateTeacher
        ///     {
        ///         "id": "d9f9b9f0-5b5a-4b9f-9b9f-0d5b5a4b9f9b",
        ///         "fullName": "Nguyễn Xuân Nhân",
        ///         "email": "nguyenxuannhan407@gmail.com",
        ///         "phone": "9876543210",
        ///         "customerId": null
        ///     }
        /// </remarks>
        /// <response code="200">Cập nhật giáo viên thành công</response>
        /// <response code="404">Không tìm thấy giáo viên</response>
        /// <response code="500">Lỗi server</response>
        [HttpPut("updateTeacher/{id}")]
        public async Task<IActionResult> UpdateTeacher([FromBody] Model.Teacher teacher, Guid id)
        {
            MapperConfiguration config = new(cfg => cfg.CreateMap<Model.Teacher, Teacher>());
            var mapper = config.CreateMapper();
            try
            {
                var existTeacher = await Task.Run(() => _teacherService.GetTeachers().FirstOrDefault(x => x.id == id));
                if (existTeacher == null)
                    return NotFound(new { status = false, message = "Teacher not found" });
                mapper.Map(teacher, existTeacher);
                await _teacherService.UpdateTeacher(existTeacher, id);
                return Ok(new { status = true, message = "Update teacher successfully" });
            }
            catch (Exception)
            {
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
        ///     DELETE /deleteTeacher
        ///     {
        ///         "id": "d9f9b9f0-5b5a-4b9f-9b9f-0d5b5a4b9f9b"
        ///     }
        /// </remarks>
        /// <response code="200">Xóa giáo viên thành công</response>
        /// <response code="404">Không tìm thấy giáo viên</response>
        /// <response code="500">Lỗi server</response>
        [HttpDelete("deleteTeacher/{id}")]
        public async Task<IActionResult> DeleteTeacher(Guid id)
        {
            try
            {
                if (await Task.Run(() => _teacherService.GetTeachers().FirstOrDefault(x => x.id == id)) == null)
                    return NotFound( new { status = false, message = "Teacher not found" });
                await _teacherService.DeleteTeacher(id);
                return Ok(new { status = true, message = "Delete teacher successfully" });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { status = false, message = "An error occurred while processing your request" });
            }
        }
    }
}
