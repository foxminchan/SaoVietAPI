using Application.Services;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using Application.Transaction;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;

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
        private readonly TransactionService _transactionService;

        /// <inheritdoc />
        public ClassController(
            ClassService classService, 
            TransactionService transactionService,
            ILogger<TeacherController> logger)
        {
            _logger = logger;
            _mapper = new MapperConfiguration(cfg => cfg.CreateMap<Models.Class, Domain.Entities.Class>()).CreateMapper();
            _classService = classService;
            _transactionService = transactionService;
        }

        private bool IsValidClass(Models.Class @class, out string? message)
        {
            if (string.IsNullOrWhiteSpace(@class.name))
            {
                message = "Class name is required";
                return false;
            }

            if (!string.IsNullOrWhiteSpace(@class.startDate) &&
                !Regex.IsMatch(@class.startDate, "^\\d{4}-\\d{2}-\\d{2}$", RegexOptions.None, TimeSpan.FromSeconds(2)))
            {
                message = "Start date must be match YYYY-MM-DD format";
                return false;
            }

            if (!string.IsNullOrWhiteSpace(@class.endDate) &&
                !Regex.IsMatch(@class.endDate, "^\\d{4}-\\d{2}-\\d{2}$", RegexOptions.None, TimeSpan.FromSeconds(2)))
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

            if (@class.teacherId != null
                && !_classService.GetTeachers().Any(x => x != null && x.id == @class.teacherId))
            {
                message = "Teacher id is not exist";
                return false;
            }

            if (@class.branchId == null
                || !_classService.GetBranches().Any(x => x != null && x.id == @class.branchId))
            {
                message = "Branch id is not exist";
                return false;
            }

            message = null;
            return true;
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
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any, VaryByQueryKeys = new[] { "classes" })]
        public ActionResult GetClasses()
        {
            try
            {
                var classes = _classService.GetClasses().ToArray();
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
        public ActionResult FindClassById([FromRoute] string? id)
        {
            if (id == null)
                return BadRequest(new { status = false, message = "Class id is required" });
            try
            {
                if (_classService.GetClasses().All(x => x.id != id))
                    return BadRequest(new { status = false, message = "Class id is not exist" });
                var @class = _classService.FindClassById(id);
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
        ///     GET /api/v1/Class/name/string
        /// </remarks>
        /// <response code="200">Tìm lớp theo tên thành công</response>
        /// <response code="400">Lỗi dữ liệu đầu vào</response>
        /// <response code="404">Không tìm thấy lớp</response>
        /// <response code="429">Request quá nhiều</response>
        /// <response code="500">Lỗi server</response>
        [HttpGet("name/{name}")]
        [AllowAnonymous]
        public ActionResult FindClassByName([FromRoute] string? name)
        {
            if (name == null)
                return BadRequest(new { status = false, message = "Name is null" });
            try
            {
                var classEntity = _classService.FindClassByName(name).ToArray();
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
        [Authorize(Policy = "Admin")]
        [Authorize(Policy = "Teacher")]
        public ActionResult FindClassByStatus([FromRoute] string? status)
        {
            if (string.IsNullOrEmpty(status))
                return BadRequest(new { status = false, message = "Status is null" });

            try
            {
                var classEntity = _classService.GetClassesByStatus(status).ToArray();
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
        [Authorize(Policy = "Admin")]
        [Authorize(Policy = "Teacher")]
        public ActionResult GetClassesByTeacherId([FromRoute] Guid? teacherId)
        {
            if (teacherId == null)
                return BadRequest(new { status = false, message = "TeacherId is null" });

            try
            {
                var classEntity = _classService.FindClassByTeacher(teacherId).ToArray();
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
        [Authorize(Policy = "Admin")]
        [Authorize(Policy = "Teacher")]
        public ActionResult GetStudentList([FromRoute] string? classId)
        {
            if (string.IsNullOrEmpty(classId))
                return BadRequest(new { status = false, message = "ClassId is null" });

            try
            {
                if (_classService.GetClasses().All(x => x.id != classId))
                    return BadRequest(new { status = false, message = "Class id is not exist" });
                var studentAmount = _classService.CountStudentInClass(classId);
                if (studentAmount == 0)
                    return NoContent();
                var studentList = _classService.GetStudentsInClass(classId);
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
        [Authorize(Policy = "Admin")]
        [Authorize(Policy = "Teacher")]
        public ActionResult AddClass([FromBody] Models.Class request)
        {
            if(!IsValidClass(request, out var message))
                return BadRequest(new { status = false, message });

            try
            {
                if (string.IsNullOrWhiteSpace(request.id)
                    || _classService.CheckClassIdExist(request.id))
                    return BadRequest(new { status = false, message = "Class id is null or existed" });
                var newClass = _mapper.Map<Domain.Entities.Class>(request);
                _transactionService.ExecuteTransaction(() => { _classService.AddClass(newClass); });
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
        /// <returns></returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT /api/v1/Class
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
        [HttpPut]
        [Authorize(Policy = "Admin")]
        [Authorize(Policy = "Teacher")]
        public ActionResult UpdateClass([FromBody] Models.Class request)
        {
            if (!IsValidClass(request, out var message))
                return BadRequest(new { status = false, message });

            try
            {
                if (string.IsNullOrWhiteSpace(request.id)
                    || !_classService.CheckClassIdExist(request.id))
                    return BadRequest(new { status = false, message = "Class id is null or not exist" });
                var classEntity = _mapper.Map<Domain.Entities.Class>(request);
                _transactionService.ExecuteTransaction(() => { _classService.UpdateClass(classEntity); });
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
        [Authorize(Policy = "Admin")]
        [Authorize(Policy = "Teacher")]
        public ActionResult DeleteClass([FromRoute] string id)
        {
            try
            {
                if (_classService.GetClasses().Any(x => x.id != id))
                    return BadRequest(new { status = false, message = "Class id is not exist" });
                _transactionService.ExecuteTransaction(() => { _classService.DeleteClass(id); });
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
        [Authorize(Policy = "Admin")]
        [Authorize(Policy = "Teacher")]
        public ActionResult DeleteStudentFromClass([FromRoute] string classId, [FromRoute] Guid studentId)
        {
            if (string.IsNullOrEmpty(classId) && studentId == Guid.Empty)
                return BadRequest(new { status = false, message = "Class id and student id are required" });

            try
            {
                if (!_classService.CheckStudentInClass(classId, studentId))
                    return BadRequest(new { status = false, message = "Class id is not exist" });
                _transactionService.ExecuteTransaction(() => {
                    _classService.DeleteStudentFromClass(
                        new Domain.Entities.ClassStudent { classId = classId, studentId = studentId });
                });
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