using Application.Services;
using AutoMapper;
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
    [Route("api/v1/[controller]")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        private readonly ILogger<TeacherController> _logger;
        private readonly IMapper _mapper;
        private readonly CourseService _courseService;

        /// <inheritdoc />
        public CourseController(CourseService courseService, ILogger<TeacherController> logger)
        {
            _logger = logger;
            _mapper = new MapperConfiguration(cfg => cfg.CreateMap<Models.Course, Domain.Entities.Course>())
                .CreateMapper();
            _courseService = courseService;
        }

        private async Task<(bool, string?)> IsValidCourse(Models.Course course)
        {
            if (string.IsNullOrEmpty(course.id))
                return (false, "Course id is required");

            if (await _courseService.CourseExists(course.id))
                return (false, "Course id already exists");

            if (!string.IsNullOrEmpty(course.categoryId) && 
                !await _courseService.IsValidCategoryId(course.categoryId))
                return (false, "Category id not exists");
            
            return (true, null);
        }

        /// <summary>
        /// Lấy danh sách khoá học
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/v1/Course/GetCourses
        /// </remarks>
        /// <response code="200">Lấy danh sách khoá học thành công</response>
        /// <response code="204">Không tìm thấy khoá học</response>
        /// <response code="429">Request quá nhiều</response>
        /// <response code="500">Lỗi server</response>
        [HttpGet("getCourses")]
        public async Task<IActionResult> GetCourses()
        {
            try
            {
                var courses = await _courseService.GetCourses();
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
        ///     GET /api/v1/Course/findByName/string
        ///     {
        ///         "name": "string"
        ///     }
        /// </remarks>
        /// <response code="200">Tìm kiếm khoá học theo tên thành công</response>
        /// <response code="404">Không tìm thấy khoá học</response>
        /// <response code="429">Request quá nhiều</response>
        /// <response code="500">Lỗi server</response>
        [HttpGet("findByNames/{name}")]
        public async Task<IActionResult> GetCoursesByNames(string? name)
        {
            try
            {
                var courses = await _courseService.GetCoursesByNames(name);
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
        ///     GET /api/v1/Course/findById/string
        ///     {
        ///         "id": "string"
        ///     }
        /// </remarks>
        /// <response code="200">Lấy khoá học theo id thành công</response>
        /// <response code="404">Không tìm thấy khoá học</response>
        /// <response code="429">Request quá nhiều</response>
        /// <response code="500">Lỗi server</response>
        [HttpGet("findById/{id}")]
        public async Task<IActionResult> GetCourseById(string? id)
        {
            try
            {
                var course = await _courseService.GetCourseById(id);
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
        ///     POST /api/v1/Course/addCourse
        ///     {
        ///         "id": "string",
        ///         "name": "string",
        ///         "description": "string",
        ///         "categoryId": "string"
        ///     }
        /// </remarks>
        /// <response code="200">Thêm khoá học thành công</response>
        /// <response code="404">Không tìm thấy khoá học</response>
        /// <response code="429">Request quá nhiều</response>
        /// <response code="500">Lỗi server</response>
        [HttpPost("addCourse")]
        public async Task<IActionResult> AddCourse([FromBody] Models.Course course)
        {
            var (isValid, message) = await IsValidCourse(course);
            if (!isValid)
                return BadRequest(new { status = false, message });

            try
            {
                var courseEntity = _mapper.Map<Domain.Entities.Course>(course);
                await _courseService.AddCourse(courseEntity);
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
        /// <param name="id">Mã khoá học</param>
        /// <returns></returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT /api/v1/Course/updateCourse/string
        ///     {
        ///         "id": "string",
        ///         "name": "string",
        ///         "description": "string",
        ///         "categoryId": "string"
        ///     }
        /// </remarks>
        /// <response code="200">Cập nhật khoá học thành công</response>
        /// <response code="404">Không tìm thấy khoá học</response>
        /// <response code="429">Request quá nhiều</response>
        /// <response code="500">Lỗi server</response>
        [HttpPut("updateCourse/{id}")]
        public async Task<IActionResult> UpdateCourse([FromBody] Models.Course course, [FromRoute] string id)
        {
            var (isValid, message) = await IsValidCourse(course);
            if (!isValid)
                return BadRequest(new { status = false, message });
            
            try
            {
                var courseEntity = _mapper.Map<Domain.Entities.Course>(course);
                await _courseService.UpdateCourse(courseEntity, id);
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
        ///     DELETE /api/v1/Course/deleteCourse/string
        ///     {
        ///         "id": "string"
        ///     }
        /// </remarks>
        /// <response code="200">Xoá khoá học thành công</response>
        /// <response code="404">Không tìm thấy khoá học</response>
        /// <response code="429">Request quá nhiều</response>
        /// <response code="500">Lỗi server</response>
        [HttpDelete("deleteCourse/{id}")]
        public async Task<IActionResult> DeleteCourse([FromRoute] string id)
        {
            if (!await _courseService.CourseExists(id))
                return NotFound(new { status = false, message = "No course id was found" });

            try
            {
                await _courseService.DeleteCourse(id);
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
