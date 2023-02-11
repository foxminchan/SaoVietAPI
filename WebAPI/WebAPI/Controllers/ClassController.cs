using Application.Services;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;

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
    [Authorize]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ClassController : ControllerBase
    {
        private readonly ILogger<TeacherController> _logger;
        private readonly IMapper _mapper;
        private readonly ClassService _classService;

        /// <inheritdoc />
        public ClassController(ClassService classService, ILogger<TeacherController> logger)
        {
            _logger = logger;
            _mapper = new MapperConfiguration(cfg => cfg.CreateMap<Models.Class, Domain.Entities.Class>()).CreateMapper();
            _classService = classService;
        }

        private async Task<(bool, string?)> IsValidClass(Models.Class @class)
        {
            if (string.IsNullOrWhiteSpace(@class.name))
                return (false, "Class name is required");

            if (!string.IsNullOrWhiteSpace(@class.startDate) &&
                !Regex.IsMatch(@class.startDate, "^\\d{4}-\\d{2}-\\d{2}$", RegexOptions.None, TimeSpan.FromSeconds(2)))
                return (false, "Start date must be match YYYY-MM-DD format");

            if (!string.IsNullOrWhiteSpace(@class.endDate) &&
                !Regex.IsMatch(@class.endDate, "^\\d{4}-\\d{2}-\\d{2}$", RegexOptions.None, TimeSpan.FromSeconds(2)))
                return (false, "End date must be match YYYY-MM-DD format");

            if (!string.IsNullOrWhiteSpace(@class.startDate) &&
                !string.IsNullOrWhiteSpace(@class.endDate) &&
                DateTime.Parse(@class.startDate) > DateTime.Parse(@class.endDate))
                return (false, "Start date must be less than end date");

            var teachers = await _classService.GetTeachers();
            if (@class.teacherId != null
                && !teachers.Any(x => x != null && x.id == @class.teacherId))
                return (false, "Teacher id is not exist");

            var branches = await _classService.GetBranches();
            if (@class.branchId == null
                || branches.Any(x => x != null && x.id == @class.branchId)) return (true, null);
            return (false, "Branch id is not exist");
        }

        /// <summary>
        /// Lấy danh sách lớp học
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/v1/Class
        /// </remarks>
        /// <response code="200">Lấy danh sách lớp học thành công</response>
        /// <response code="204">Không có lớp học nào</response>
        /// <response code="429">Request quá nhiều</response>
        /// <response code="500">Lỗi server</response>
        [HttpGet]
        [AllowAnonymous]
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
        /// Tìm kiếm lớp theo mã lớp
        /// </summary>
        /// <param name="id">Mã lớp</param>
        /// <returns></returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/v1/Class/string
        /// </remarks>
        /// <response code="200">Lấy lớp học thành công</response>
        /// <response code="400">Mã lớp không hợp lệ</response>
        /// <response code="404">Không tìm thấy lớp học</response>
        /// <response code="429">Request quá nhiều</response>
        /// <response code="500">Lỗi server</response>
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> FindClassById([FromRoute] string? id)
        {
            if (id == null)
                return BadRequest(new { status = false, message = "Class id is required" });
            try
            {
                var classes = await Task.Run(_classService.GetClasses);
                if (classes.All(x => x.id != id))
                    return BadRequest(new { status = false, message = "Class id is not exist" });
                var @class = await Task.Run(() => _classService.FindClassById(id));
                return @class != null
                    ? Ok(new { status = true, message = "Get data successfully", data = @class })
                    : NotFound(new { status = false, message = "Class not found" });
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while getting class by id");
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
        ///     GET /api/v1/Class/string
        /// </remarks>
        /// <response code="200">Tìm lớp theo tên thành công</response>
        /// <response code="400">Lỗi dữ liệu đầu vào</response>
        /// <response code="404">Không tìm thấy lớp</response>
        /// <response code="429">Request quá nhiều</response>
        /// <response code="500">Lỗi server</response>
        [HttpGet("{name}")]
        [AllowAnonymous]
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
        ///     GET /api/v1/Class/status/string
        /// </remarks>
        /// <response code="200">Tìm lớp theo trạng thái thành công</response>
        /// <response code="400">Lỗi dữ liệu đầu vào</response>
        /// <response code="401">Không có quyền</response>
        /// <response code="204">Không tìm thấy lớp</response>
        /// <response code="429">Request quá nhiều</response>
        /// <response code="500">Lỗi server</response>
        [HttpGet("status/{status:regex(Expired|Active|Upcoming)}")]
        [Authorize]
        public async Task<IActionResult> FindClassByStatus([FromRoute] string? status)
        {
            if (string.IsNullOrEmpty(status))
                return BadRequest(new { status = false, message = "Status is null" });

            try
            {
                var classEntity = await Task.Run(() => _classService.GetClassesByStatus(status));
                return !classEntity.Any()
                    ? NotFound(new { status = false, message = "No classes found with the given status" })
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
        ///     GET /api/v1/Class/teacher/guid
        /// </remarks>
        /// <response code="200">Liệt kê lớp theo giáo viên thành công</response>
        /// <response code="400">Lỗi dữ liệu đầu vào</response>
        /// <response code="401">Không có quyền</response>
        /// <response code="404">Không tìm thấy lớp</response>
        /// <response code="429">Request quá nhiều</response>
        /// <response code="500">Lỗi server</response>
        [HttpGet("teacher/{teacherId:Guid}")]
        [Authorize]
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
        /// Lấy danh sách học viên trong lớp
        /// </summary>
        /// <param name="classId">Mã lớp</param>
        /// <returns></returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/v1/Class/student/string
        /// </remarks>
        /// <response code="200">Lấy danh sách học viên trong lớp thành công</response>
        /// <response code="204">Không có dữ liệu</response>
        /// <response code="400">Lỗi dữ liệu đầu vào</response>
        /// <response code="401">Không có quyền</response>
        /// <response code="429">Request quá nhiều</response>
        /// <response code="500">Lỗi server</response>
        [HttpGet("student/{classId}")]
        [Authorize]
        public async Task<IActionResult> GetStudentList([FromRoute] string? classId)
        {
            if (string.IsNullOrEmpty(classId))
                return BadRequest(new { status = false, message = "ClassId is null" });

            try
            {
                var classes = await Task.Run(_classService.GetClasses);
                if (classes.All(x => x.id != classId))
                    return BadRequest(new { status = false, message = "Class id is not exist" });
                var studentAmount = await Task.Run(() => _classService.CountStudentInClass(classId));
                if (studentAmount == 0)
                    return NoContent();
                var studentList = await Task.Run(() => _classService.GetStudentsInClass(classId));
                return Ok(new { status = true, message = "Get student list successfully", amount = studentAmount, data = studentList });

            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while getting student list");
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
        ///     POST /api/v1/Class
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
        /// <response code="401">Không có quyền</response>
        /// <response code="429">Request quá nhiều</response>
        /// <response code="500">Lỗi server</response>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddClass([FromBody] Models.Class request)
        {
            var (isValid, message) = await Task.Run(() => IsValidClass(request));
            if (!isValid)
                return BadRequest(new { status = false, message });

            try
            {
                if (string.IsNullOrWhiteSpace(request.id)
                    || await Task.Run(() => _classService.CheckClassIdExist(request.id)))
                    return BadRequest(new { status = false, message = "Class id is null or existed" });
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
        ///     PUT /api/v1/Class/string
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
        /// <response code="401">Không có quyền</response>
        /// <response code="429">Request quá nhiều</response>
        /// <response code="500">Lỗi server</response>
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateClass([FromBody] Models.Class request, [FromRoute] string id)
        {
            var (isValid, message) = await Task.Run(() => IsValidClass(request));
            if (!isValid)
                return BadRequest(new { status = false, message });

            try
            {
                if (string.IsNullOrWhiteSpace(request.id)
                    || !await Task.Run(() => _classService.CheckClassIdExist(request.id)))
                    return BadRequest(new { status = false, message = "Class id is null or not exist" });
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
        ///     DELETE /api/v1/Class/string
        /// </remarks>
        /// <response code="200">Xóa lớp học thành công</response>
        /// <response code="400">Lỗi dữ liệu đầu vào</response>
        /// <response code="429">Request quá nhiều</response>
        /// <response code="500">Lỗi server</response>
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteClass([FromRoute] string id)
        {
            try
            {
                var classes = await Task.Run(_classService.GetClasses);
                if (classes.Any(x => x.id != id))
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

        /// <summary>
        /// Xóa học viên khỏi lớp học
        /// </summary>
        /// <param name="classId">Mã lớp</param>
        /// <param name="studentId">Mã học viên</param>
        /// <returns></returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     DELETE /api/v1/Class/string/student/guid
        /// </remarks>
        /// <response code="200">Xóa học viên khỏi lớp học thành công</response>
        /// <response code="400">Lỗi dữ liệu đầu vào</response>
        /// <response code="401">Không có quyền</response>
        /// <response code="429">Request quá nhiều</response>
        /// <response code="500">Lỗi server</response>
        [HttpDelete("{classId}/student/{studentId:guid}")]
        [Authorize]
        public async Task<IActionResult> DeleteStudentFromClass([FromRoute] string classId, [FromRoute] Guid studentId)
        {
            if (string.IsNullOrEmpty(classId) && studentId == Guid.Empty)
                return BadRequest(new { status = false, message = "Class id and student id are required" });

            try
            {
                if (!await Task.Run(() => _classService.CheckStudentInClass(classId, studentId)))
                    return BadRequest(new { status = false, message = "Class id is not exist" });
                await Task.Run(() => _classService.DeleteStudentFromClass(new Domain.Entities.ClassStudent { classId = classId, studentId = studentId }));
                return Ok(new { status = true, message = "Delete student from class successfully" });
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while deleting student from class");
                return StatusCode(StatusCodes.Status500InternalServerError, new { status = false, message = "An error occurred while processing your request" });
            }
        }
    }
}