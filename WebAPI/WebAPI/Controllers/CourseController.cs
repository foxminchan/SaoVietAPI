using Application.Services;
using Application.Transaction;
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
    /// Quản lý khoá học
    /// </summary>
    [Authorize]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        private readonly ILogger<TeacherController> _logger;
        private readonly IMapper _mapper;
        private readonly CourseService _courseService;
        private readonly TransactionService _transactionService;

        /// <inheritdoc />
        public CourseController(
            CourseService courseService,
            TransactionService transactionService,
            ILogger<TeacherController> logger)
        {
            _logger = logger;
            _mapper = new MapperConfiguration(cfg => cfg.CreateMap<Models.Course, Domain.Entities.Course>())
                .CreateMapper();
            _courseService = courseService;
            _transactionService = transactionService;
        }

        private bool IsValidCourse(Models.Course course, out string? message)
        {
            if (string.IsNullOrEmpty(course.id))
            {
                message = "Course id is required";
                return false;
            }

            if (_courseService.CourseExists(course.id))
            {
                message = "Course id already exists";
                return false;
            }

            if (!string.IsNullOrEmpty(course.categoryId) &&
                !_courseService.IsValidCategoryId(course.categoryId))
            {
                message = "Category id not exists";
                return false;
            }

            message = null;
            return true;
        }

        /// <summary>
        /// Lấy danh sách khoá học
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/v1/Course
        /// </remarks>
        /// <response code="200">Lấy danh sách khoá học thành công</response>
        /// <response code="204">Không tìm thấy khoá học</response>
        /// <response code="408">Quá thời gian yêu cầu</response>
        /// <response code="429">Request quá nhiều</response>
        /// <response code="500">Lỗi server</response>
        [HttpGet]
        [AllowAnonymous]
        [ResponseCache(Duration = 30, Location = ResponseCacheLocation.Any, VaryByQueryKeys = new[] { "courses" })]
        public ActionResult GetCourses()
        {
            try
            {
                var courses = _courseService.GetCourses().ToArray();
                return courses.Any()
                    ? Ok(new { status = true, message = "Get data successfully", data = courses })
                    : NoContent();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while getting courses");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { status = false, message = "An error occurred while processing your request" });
            }
        }

        /// <summary>
        /// Tìm kiếm khoá học theo tên
        /// </summary>
        /// <param name="name">Tên khoá học</param>
        /// <returns></returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/v1/Course/name/string
        /// </remarks>
        /// <response code="200">Tìm kiếm khoá học theo tên thành công</response>
        /// <response code="404">Không tìm thấy khoá học</response>
        /// <response code="408">Quá thời gian yêu cầu</response>
        /// <response code="429">Request quá nhiều</response>
        /// <response code="500">Lỗi server</response>
        [HttpGet("name/{name}")]
        [AllowAnonymous]
        public ActionResult GetCoursesByNames([FromRoute] string? name)
        {
            try
            {
                var courses = _courseService.GetCoursesByNames(name).ToArray();
                return courses.Any()
                    ? Ok(new { status = true, message = "Get data successfully", data = courses })
                    : NoContent();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while getting courses");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { status = false, message = "An error occurred while processing your request" });
            }
        }

        /// <summary>
        /// Lấy khoá học theo id
        /// </summary>
        /// <param name="id">Mã khoá học</param>
        /// <returns></returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/v1/Course/string
        /// </remarks>
        /// <response code="200">Lấy khoá học theo id thành công</response>
        /// <response code="404">Không tìm thấy khoá học</response>
        /// <response code="408">Quá thời gian yêu cầu</response>
        /// <response code="429">Request quá nhiều</response>
        /// <response code="500">Lỗi server</response>
        [HttpGet("{id}")]
        [AllowAnonymous]
        public ActionResult GetCourseById([FromRoute] string? id)
        {
            try
            {
                var course = _courseService.GetCourseById(id);
                return course != null
                    ? Ok(new { status = true, message = "Get data successfully", data = course })
                    : NotFound(new { status = false, message = "No course was found" });
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while getting courses");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { status = false, message = "An error occurred while processing your request" });
            }
        }

        /// <summary>
        /// Thêm khoá học
        /// </summary>
        /// <param name="course">Đối tượng khoá học</param>
        /// <returns></returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/v1/Course
        ///     {
        ///         "id": "string",
        ///         "name": "string",
        ///         "description": "string",
        ///         "categoryId": "string"
        ///     }
        /// </remarks>
        /// <response code="200">Thêm khoá học thành công</response>
        /// <response code="401">Không có quyền</response>
        /// <response code="404">Không tìm thấy khoá học</response>
        /// <response code="408">Quá thời gian yêu cầu</response>
        /// <response code="429">Request quá nhiều</response>
        /// <response code="500">Lỗi server</response>
        [HttpPost]
        [Authorize(Policy = "Admin")]
        [Authorize(Policy = "Teacher")]
        public ActionResult AddCourse([FromBody] Models.Course course)
        {
            if (!IsValidCourse(course, out var message))
                return BadRequest(new { status = false, message });

            try
            {
                var courseEntity = _mapper.Map<Domain.Entities.Course>(course);
                _transactionService.ExecuteTransaction(() => { _courseService.AddCourse(courseEntity); });
                return Ok(new { status = true, message = "Add course successfully" });
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while adding course");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { status = false, message = "An error occurred while processing your request" });
            }
        }

        /// <summary>
        /// Cập nhật khoá học
        /// </summary>
        /// <param name="course">Đối tượng khoá học</param>
        /// <returns></returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT /api/v1/Course
        ///     {
        ///         "id": "string",
        ///         "name": "string",
        ///         "description": "string",
        ///         "categoryId": "string"
        ///     }
        /// </remarks>
        /// <response code="200">Cập nhật khoá học thành công</response>
        /// <response code="401">Không có quyền</response>
        /// <response code="404">Không tìm thấy khoá học</response>
        /// <response code="408">Quá thời gian yêu cầu</response>
        /// <response code="429">Request quá nhiều</response>
        /// <response code="500">Lỗi server</response>
        [HttpPut]
        [Authorize(Policy = "Admin")]
        [Authorize(Policy = "Teacher")]
        public ActionResult UpdateCourse([FromBody] Models.Course course)
        {
            if (!IsValidCourse(course, out var message))
                return BadRequest(new { status = false, message });

            try
            {
                var courseEntity = _mapper.Map<Domain.Entities.Course>(course);
                _transactionService.ExecuteTransaction(() => { _courseService.UpdateCourse(courseEntity); });
                return Ok(new { status = true, message = "Update course successfully" });
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while updating course");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { status = false, message = "An error occurred while processing your request" });
            }
        }

        /// <summary>
        /// Xoá khoá học
        /// </summary>
        /// <param name="id">Mã khoá học</param>
        /// <returns></returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     DELETE /api/v1/Course/string
        /// </remarks>
        /// <response code="200">Xoá khoá học thành công</response>
        /// <response code="401">Không có quyền</response>
        /// <response code="404">Không tìm thấy khoá học</response>
        /// <response code="408">Quá thời gian yêu cầu</response>
        /// <response code="429">Request quá nhiều</response>
        /// <response code="500">Lỗi server</response>
        [HttpDelete("{id}")]
        [Authorize(Policy = "Admin")]
        [Authorize(Policy = "Teacher")]
        public ActionResult DeleteCourse([FromRoute] string id)
        {
            try
            {
                if (!_courseService.CourseExists(id))
                    return NotFound(new { status = false, message = "No course id was found" });
                _transactionService.ExecuteTransaction(() => { _courseService.DeleteCourse(id); });
                return Ok(new { status = true, message = "Delete course successfully" });
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while deleting course");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { status = false, message = "An error occurred while processing your request" });
            }
        }
    }
}
