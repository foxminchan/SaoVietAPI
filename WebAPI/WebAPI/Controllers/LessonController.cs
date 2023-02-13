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
    /// Quản lý bài học
    /// </summary>
    [Authorize]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class LessonController : ControllerBase
    {
        private readonly ILogger<TeacherController> _logger;
        private readonly IMapper _mapper;
        private readonly LessonService _lessonService;
        private readonly IRabbitMqService _rabbitMqService;

        /// <inheritdoc />
        public LessonController(LessonService lessonService, ILogger<TeacherController> logger)
        {
            _logger = logger;
            _rabbitMqService = new RabbitMqService("lessonQueue");
            _mapper = new MapperConfiguration(cfg => cfg.CreateMap<Models.Lesson, Domain.Entities.Lesson>()).CreateMapper();
            _lessonService = lessonService;
        }

        private async Task<(bool, string?)> IsValidLesson(Models.Lesson lesson)
        {
            if (string.IsNullOrEmpty(lesson.id))
                return (false, "Id is required");
            if (string.IsNullOrEmpty(lesson.name))
                return (false, "Name is required");
            if (lesson.courseId != null && !await _lessonService.IsCourseExists(lesson.courseId))
                return (false, "Course id is not exists");
            return (true, null);
        }

        /// <summary>
        /// Lấy danh sách bài học
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/v1/Lesson
        /// </remarks>
        /// <response code="200">Lấy danh sách bài học thành công</response>
        /// <response code="204">Không có bài học nào</response>
        /// <response code="429">Request quá nhiều</response>
        /// <response code="500">Lỗi server</response>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetLessons()
        {
            try
            {
                var lessons = await Task.Run(_lessonService.GetAllLesson);
                return lessons.Any()
                    ? Ok(new { status = true, message = "Get data successfully", data = lessons })
                    : NoContent();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while getting branch");
                return StatusCode(StatusCodes.Status500InternalServerError, new { status = false, message = "An error occurred while processing your request" });
            }
        }

        /// <summary>
        /// Lấy bài học theo tên
        /// </summary>
        /// <param name="name">Tên bài học</param>
        /// <returns></returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/v1/Lesson/name/string
        /// </remarks>
        /// <response code="200">Lấy bài học thành công</response>
        /// <response code="204">Không có bài học nào</response>
        /// <response code="429">Request quá nhiều</response>
        /// <response code="500">Lỗi server</response>
        [HttpGet("name/{name}")]
        [AllowAnonymous]
        public async Task<IActionResult> FindByName([FromRoute] string name)
        {
            try
            {
                var lessons = await Task.Run(() => _lessonService.GetByNames(name));
                return lessons.Any()
                    ? Ok(new { status = true, message = "Get data successfully", data = lessons })
                    : NoContent();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while getting branch");
                return StatusCode(StatusCodes.Status500InternalServerError, new { status = false, message = "An error occurred while processing your request" });
            }
        }

        /// <summary>
        /// Tìm kiếm bài học theo mã
        /// </summary>
        /// <param name="id">Mã bài học</param>
        /// <returns></returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/v1/Lesson/string
        /// </remarks>
        /// <response code="200">Lấy bài học thành công</response>
        /// <response code="404">Không có bài học nào</response>
        /// <response code="429">Request quá nhiều</response>
        /// <response code="500">Lỗi server</response>
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> FindById([FromRoute] string id)
        {
            try
            {
                var lesson = await Task.Run(() => _lessonService.GetLessonById(id));
                return lesson != null
                    ? Ok(new { status = true, message = "Get data successfully", data = lesson })
                    : NotFound(new { status = false, message = "No lesson was found" });
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while getting branch");
                return StatusCode(StatusCodes.Status500InternalServerError, new { status = false, message = "An error occurred while processing your request" });
            }
        }

        /// <summary>
        /// Thêm bài học
        /// </summary>
        /// <param name="lesson">Đối tượng bài học</param>
        /// <returns></returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/v1/Lesson/addLesson
        ///     {
        ///         "id": "string",
        ///         "name": "string",
        ///         "description": "string",
        ///         "courseId": "string"
        ///     }
        /// </remarks>
        /// <response code="200">Thêm bài học thành công</response>
        /// <response code="400">Lỗi dữ liệu đầu vào</response>
        /// <response code="401">Không có quyền</response>
        /// <response code="429">Request quá nhiều</response>
        /// <response code="500">Lỗi server</response>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddLesson([FromBody] Models.Lesson lesson)
        {
            var (isValid, message) = await Task.Run(() => IsValidLesson(lesson));
            if (!isValid)
                return BadRequest(new { status = false, message });
            try
            {
                if (lesson.id != null && await Task.Run(() => _lessonService.GetLessonById(lesson.id)) != null)
                    return BadRequest(new { status = false, message = "Lesson id is null or exists" });
                var lessonEntity = _mapper.Map<Domain.Entities.Lesson>(lesson);
                await Task.Run(() => _lessonService.AddLesson(lessonEntity));
                _rabbitMqService.SendMessage(lesson);
                return Ok(new { status = true, message = "Add lesson successfully" });
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while getting branch");
                return StatusCode(StatusCodes.Status500InternalServerError, new { status = false, message = "An error occurred while processing your request" });
            }
        }

        /// <summary>
        /// Cập nhật bài học
        /// </summary>
        /// <param name="id">Mã bài học</param>
        /// <param name="lesson">Đối tượng bài học</param>
        /// <returns></returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT /api/v1/Lesson/string
        ///     {
        ///         "id": "string",
        ///         "name": "string",
        ///         "description": "string",
        ///         "courseId": "string"
        ///     }
        /// </remarks>
        /// <response code="200">Cập nhật bài học thành công</response>
        /// <response code="400">Lỗi dữ liệu đầu vào</response>
        /// <response code="401">Không có quyền</response>
        /// <response code="404">Không có bài học nào</response>
        /// <response code="429">Request quá nhiều</response>
        /// <response code="500">Lỗi server</response>
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateLesson([FromRoute] string id, [FromBody] Models.Lesson lesson)
        {
            var (isValid, message) = await Task.Run(() => IsValidLesson(lesson));
            if (!isValid)
                return BadRequest(new { status = false, message });
            try
            {
                var lessonEntity = await Task.Run(() => _lessonService.GetLessonById(id));
                if (lessonEntity == null)
                    return NotFound(new { status = false, message = "Lesson not found" });
                lessonEntity = _mapper.Map<Domain.Entities.Lesson>(lesson);
                await Task.Run(() => _lessonService.UpdateLesson(lessonEntity, id));
                return Ok(new { status = true, message = "Update lesson successfully" });
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while getting branch");
                return StatusCode(StatusCodes.Status500InternalServerError, new { status = false, message = "An error occurred while processing your request" });
            }
        }

        /// <summary>
        /// Xóa bài học
        /// </summary>
        /// <param name="id">Mã bài học</param>
        /// <returns></returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     DELETE /api/v1/Lesson/string
        /// </remarks>
        /// <response code="200">Xóa bài học thành công</response>
        /// <response code="400">Lỗi dữ liệu đầu vào</response>
        /// <response code="401">Không có quyền</response>
        /// <response code="404">Không có bài học nào</response>
        /// <response code="429">Request quá nhiều</response>
        /// <response code="500">Lỗi server</response>
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteLesson([FromRoute] string id)
        {
            try
            {
                var lessonEntity = await Task.Run(() => _lessonService.GetLessonById(id));
                if (lessonEntity == null)
                    return NotFound(new { status = false, message = "Lesson not found" });
                await Task.Run(() => _lessonService.DeleteLesson(lessonEntity));
                return Ok(new { status = true, message = "Delete lesson successfully" });
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while getting branch");
                return StatusCode(StatusCodes.Status500InternalServerError, new { status = false, message = "An error occurred while processing your request" });
            }
        }
    }
}
