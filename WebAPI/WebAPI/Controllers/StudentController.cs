using Application.Services;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using Application.Message;

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
    /// Quản lý học viên
    /// </summary>
    [Authorize]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly ILogger<TeacherController> _logger;
        private readonly IMapper _mapper;
        private readonly StudentService _studentService;
        private readonly IRabbitMqService _rabbitMqService;

        /// <inheritdoc />
        public StudentController(StudentService studentService, ILogger<TeacherController> logger)
        {
            _logger = logger;
            _rabbitMqService = new RabbitMqService("studentQueue");
            _mapper = new MapperConfiguration(cfg => cfg.CreateMap<Models.Student, Domain.Entities.Student>()).CreateMapper();
            _studentService = studentService;
        }

        private static async Task<(bool, string?)> IsValidStudent(Models.Student student)
        {
            await Task.Delay(0);

            if (string.IsNullOrEmpty(student.fullName) &&
                string.IsNullOrEmpty(student.dob) &&
                string.IsNullOrEmpty(student.phone))
                return (false, "Full name, date of birth and phone number are required");

            if (!string.IsNullOrWhiteSpace(student.email) &&
                !Regex.IsMatch(student.email, @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", RegexOptions.None, TimeSpan.FromSeconds(2)))
                return (false, "Email is invalid");

            if (!string.IsNullOrWhiteSpace(student.phone) &&
                !Regex.IsMatch(student.phone, @"^([0-9]{10})$", RegexOptions.None, TimeSpan.FromSeconds(2)))
                return (false, "Phone is invalid");

            if (string.IsNullOrWhiteSpace(student.dob) ||
                Regex.IsMatch(student.dob, "^\\d{4}-\\d{2}-\\d{2}$", RegexOptions.None, TimeSpan.FromSeconds(2))) return (true, null);
            return (false, "Start date must be match YYYY-MM-DD format");

        }

        /// <summary>
        /// Lấy danh sách học viên
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/v1/Student
        /// </remarks>
        /// <response code="200">Lấy danh sách học viên thành công</response>
        /// <response code="204">Không có học viên nào</response>
        /// <response code="401">Không có quyền</response>
        /// <response code="429">Request quá nhiều</response>
        /// <response code="500">Lỗi server</response>
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetStudents()
        {
            try
            {
                var students = await Task.Run(_studentService.GetStudents);
                return students.Any()
                    ? Ok(new { status = true, message = "Get data successfully", data = students })
                    : NoContent();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while getting all teacher");
                return StatusCode(StatusCodes.Status500InternalServerError, new { status = false, message = "An error occurred while processing your request" });
            }
        }

        /// <summary>
        /// Tìm kiếm học viên theo tên
        /// </summary>
        /// <param name="name">Tên học viên</param>
        /// <returns></returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/v1/Student/name/string
        /// </remarks>
        /// <response code="200">Tìm kiếm học viên thành công</response>
        /// <response code="204">Không có học viên nào</response>
        /// <response code="401">Không có quyền</response>
        /// <response code="429">Request quá nhiều</response>
        /// <response code="500">Lỗi server</response>
        [HttpGet("name/{name}")]
        [Authorize]
        public async Task<IActionResult> GetStudentsByName([FromRoute] string? name)
        {
            try
            {
                var students = await Task.Run(() => _studentService.GetStudentsByNames(name));
                return students.Any()
                    ? Ok(new { status = true, message = "Get data successfully", data = students })
                    : NoContent();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while getting all teacher");
                return StatusCode(StatusCodes.Status500InternalServerError, new { status = false, message = "An error occurred while processing your request" });
            }
        }

        /// <summary>
        /// Tìm kiếm học viên theo số điện thoại
        /// </summary>
        /// <param name="phone">Số điện thoại học viên</param>
        /// <returns></returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/v1/Student/phone/number
        /// </remarks>
        /// <response code="200">Tìm kiếm học viên thành công</response>
        /// <response code="204">Không có học viên nào</response>
        /// <response code="401">Không có quyền</response>
        /// <response code="429">Request quá nhiều</response>
        /// <response code="500">Lỗi server</response>
        [HttpGet("phone/{phone:regex(^\\d{{10}}$)}")]
        [Authorize]
        public async Task<IActionResult> GetStudentsByPhone([FromRoute] string? phone)
        {
            try
            {
                var students = await Task.Run(() => _studentService.GetStudentsByPhone(phone));
                return students.Any()
                    ? Ok(new { status = true, message = "Get data successfully", data = students })
                    : NoContent();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while getting all teacher");
                return StatusCode(StatusCodes.Status500InternalServerError, new { status = false, message = "An error occurred while processing your request" });
            }
        }

        /// <summary>
        /// Tìm kiếm học viên theo mã học viên
        /// </summary>
        /// <param name="id">Mã học viên</param>
        /// <returns></returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/v1/Student/guid
        /// </remarks>
        /// <response code="200">Tìm kiếm học viên thành công</response>
        /// <response code="204">Không có học viên nào</response>
        /// <response code="429">Request quá nhiều</response>
        /// <response code="500">Lỗi server</response>
        [HttpGet("{id:guid}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetStudentById([FromRoute] Guid? id)
        {
            try
            {
                var student = await Task.Run(() => _studentService.GetStudentById(id));
                return student != null
                    ? Ok(new { status = true, message = "Get data successfully", data = student })
                    : NoContent();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while getting all teacher");
                return StatusCode(StatusCodes.Status500InternalServerError, new { status = false, message = "An error occurred while processing your request" });
            }
        }

        /// <summary>
        /// Lấy danh sách lớp học của học viên
        /// </summary>
        /// <param name="id">Mã học viên</param>
        /// <returns></returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/v1/Student/class/guid
        /// </remarks>
        /// <response code="200">Lấy danh sách lớp học của học viên thành công</response>
        /// <response code="204">Không có lớp học nào</response>
        /// <response code="400">Lỗi dữ liệu đầu vào</response>
        /// <response code="401">Không có quyền</response>
        /// <response code="429">Request quá nhiều</response>
        /// <response code="500">Lỗi server</response>
        [HttpGet("class/{id:guid}")]
        [Authorize]
        public async Task<IActionResult> GetClassByStudentId([FromRoute] Guid? id)
        {
            if (id == null)
                return BadRequest(new { status = false, message = "Id is required" });
            try
            {
                var count = await Task.Run(() => _studentService.CountClassByStudent(id));
                if (count == 0)
                    return NoContent();
                var classes = await Task.Run(() => _studentService.GetClassesByStudentId(id));
                return Ok(new { status = true, message = "Get data successfully", amount = count, data = classes });
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while getting all teacher");
                return StatusCode(StatusCodes.Status500InternalServerError, new { status = false, message = "An error occurred while processing your request" });
            }
        }

        /// <summary>
        /// Thêm mới học viên
        /// </summary>
        /// <param name="student">Đối tượng học viên</param>
        /// <returns></returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/v1/Student
        ///     {
        ///         "id": guid,
        ///         "fullName": string,
        ///         "dob": "yyyy-MM-dd",
        ///         "email": string,
        ///         "phone": number
        ///     }
        /// </remarks>
        /// <response code="200">Thêm mới học viên thành công</response>
        /// <response code="400">Thông tin học viên không hợp lệ</response>
        /// <response code="401">Không có quyền</response>
        /// <response code="429">Request quá nhiều</response>
        /// <response code="500">Lỗi server</response>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddStudent([FromBody] Models.Student student)
        {
            var (isValid, message) = await Task.Run(() => IsValidStudent(student));
            if (!isValid)
                return BadRequest(new { status = false, message });

            try
            {
                var newStudent = _mapper.Map<Domain.Entities.Student>(student);
                newStudent.id = Guid.NewGuid();
                await Task.Run(() => _studentService.AddStudent(newStudent));
                _rabbitMqService.SendMessage(student);
                return Ok(new { status = true, message = "Add student successfully" });
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while getting all teacher");
                return StatusCode(StatusCodes.Status500InternalServerError, new { status = false, message = "An error occurred while processing your request" });
            }
        }

        /// <summary>
        /// Thêm học viên vào lớp học
        /// </summary>
        /// <param name="studentId">Mã học viên</param>
        /// <param name="classId">Mã lớp</param>
        /// <returns></returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT /api/v1/Student/guid/string
        /// </remarks>
        /// <response code="200">Thêm học viên vào lớp học thành công</response>
        /// <response code="400">Thông tin học viên không hợp lệ</response>
        /// <response code="401">Không có quyền</response>
        /// <response code="429">Request quá nhiều</response>
        /// <response code="500">Lỗi server</response>
        [HttpPost("{studentId:guid}/{classId}")]
        [Authorize]
        public async Task<IActionResult> AddStudentToClass([FromRoute] Guid? studentId, [FromRoute] string classId)
        {
            if (studentId == null || string.IsNullOrEmpty(classId))
                return BadRequest(new { status = false, message = "Student id and class id are required" });

            try
            {
                if (!await Task.Run(() => _studentService.CheckStudentExists(studentId.Value)))
                    return BadRequest(new { status = false, message = "Student id is not exists" });
                if (!await Task.Run(() => _studentService.CheckClassExists(classId)))
                    return BadRequest(new { status = false, message = "Class id is not exists" });
                if (await Task.Run(() => _studentService.IsAlreadyInClass(studentId.Value, classId)))
                    return BadRequest(new { status = false, message = "Student is already in class" });
                await Task.Run(() => _studentService.AddClassStudent(new Domain.Entities.ClassStudent { classId = classId, studentId = studentId.Value }));
                _rabbitMqService.SendMessage(new { studentId = studentId.Value, classId });
                return Ok(new { status = true, message = "Add student to class successfully" });
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while adding student to class");
                return StatusCode(StatusCodes.Status500InternalServerError, new { status = false, message = "An error occurred while processing your request" });
            }
        }


        /// <summary>
        /// Cập nhật thông tin học viên
        /// </summary>
        /// <param name="student">Đối tượng học viên</param>
        /// <param name="id">Mã học viên</param>
        /// <returns></returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT /api/v1/Student/guid
        ///     {
        ///         "fullName": "string",
        ///         "dob": "yyyy-MM-dd",
        ///         "email": "string",
        ///         "phone": "string"
        ///     }
        /// </remarks>
        /// <response code="200">Cập nhật thông tin học viên thành công</response>
        /// <response code="400">Thông tin học viên không hợp lệ</response>
        /// <response code="401">Không có quyền</response>
        /// <response code="429">Request quá nhiều</response>
        /// <response code="500">Lỗi server</response>
        [HttpPut("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> UpdateStudent([FromBody] Models.Student student, [FromRoute] Guid id)
        {
            try
            {
                var existStudent = await Task.Run(() => _studentService.GetStudentById(id));
                if (existStudent == null)
                    return BadRequest(new { status = false, message = "Student not found" });
                var (isValid, message) = await Task.Run(() => IsValidStudent(student));
                if (!isValid)
                    return BadRequest(new { status = false, message });
                var newStudent = _mapper.Map<Domain.Entities.Student>(student);
                newStudent.id = id;
                await Task.Run(() => _studentService.UpdateStudent(newStudent, id));
                return Ok(new { status = true, message = "Update student successfully" });
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while getting all teacher");
                return StatusCode(StatusCodes.Status500InternalServerError, new { status = false, message = "An error occurred while processing your request" });
            }
        }

        /// <summary>
        /// Xóa học viên
        /// </summary>
        /// <param name="id">Mã học viên</param>
        /// <returns></returns>
        /// <remarks>
        /// Sample request
        ///
        ///     DELETE /api/v1/Student/guid
        /// </remarks>
        /// <response code="200">Xóa học viên thành công</response>
        /// <response code="400">Học viên không tồn tại</response>
        /// <response code="401">Không có quyền</response>
        /// <response code="429">Request quá nhiều</response>
        /// <response code="500">Lỗi server</response>
        [HttpDelete("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> DeleteStudent([FromRoute] Guid id)
        {
            try
            {
                var existStudent = await Task.Run(() => _studentService.GetStudentById(id));
                if (existStudent == null)
                    return BadRequest(new { status = false, message = "Student not found" });
                await Task.Run(() => _studentService.DeleteStudent(id));
                return Ok(new { status = true, message = "Delete student successfully" });
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while getting all teacher");
                return StatusCode(StatusCodes.Status500InternalServerError, new { status = false, message = "An error occurred while processing your request" });
            }
        }
    }
}
