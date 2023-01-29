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

    /// <summary>
    /// Quản lý lớp học
    /// </summary>
    [Route("api/v1/[controller]")]
    [ApiController]
    public partial class ClassController : ControllerBase
    {
        private readonly ClassService _classService;

        [GeneratedRegex("^\\d{4}-\\d{2}-\\d{2}$")]
        private static partial Regex DateFormatRegex();

        /// <inheritdoc />
        public ClassController(ClassService classService)
        {
            _classService = classService;
        }

        /// <summary>
        /// Lấy danh sách lớp học
        /// </summary>
        /// <returns>Danh sách lớp học</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /getClasses
        /// </remarks>
        /// <response code="200">Lấy danh sách lớp học thành công</response>
        /// <response code="204">Không có lớp học nào</response>
        /// <response code="500">Lỗi server</response>
        [HttpGet("getClasses")]
        public async Task<IActionResult> GetClasses()
        {
            try
            {
                var classes = await Task.Run(_classService.GetClasses);
                if (classes.Count == 0)
                    return NoContent();
                return Ok(new { status = true, message = "Get data successfully", data = classes });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { status = false, message = "An error occurred while processing your request" });
            }
        }

        /// <summary>
        /// Thêm lớp học
        /// </summary>
        /// <param name="request">Đối tượng lớp học</param>
        /// <returns>Lớp học mới được tạo</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/v1/class/getClasses
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
        /// <response code="500">Lỗi server</response>
        [HttpPost("addClass")]
        public async Task<IActionResult> AddClass([FromBody] Model.Class request)
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<Model.Class, Class>());

            try
            {
                var classEntity = config.CreateMapper().Map<Class>(request);

                if (await Task.Run(() => _classService.GetClasses().Any(x => x.id == classEntity.id)))
                    return BadRequest(new { status = false, message = "Class id is exist" });
                if (classEntity.startDate != null && !DateFormatRegex().IsMatch(classEntity.startDate))
                    return BadRequest(new { status = false, message = "Start date must be match YYYY-MM-DD format" });
                if (classEntity.endDate != null && classEntity.startDate != null && DateTime.Parse(classEntity.startDate) > DateTime.Parse(classEntity.endDate))
                        return BadRequest(new { status = false, message = "Start date must be less than end date" });
                if (await Task.Run(() => _classService.GetTeachers().Any(x => x != null && x.id == classEntity.teacherId)) == false)
                    return BadRequest(new { status = false, message = "Teacher id is not exist" });
                
                await Task.Run(() => _classService.AddClass(classEntity));
                return Ok(new { status = true, message = "Add class successfully", data = classEntity });
            }
            catch (Exception)
            {
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
        ///     PUT /api/v1/class/updateClass/string
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
        /// <response code="500">Lỗi server</response>
        [HttpPut("updateClass/{id}")]
        public async Task<IActionResult> UpdateClass([FromBody] Model.Class request, [FromRoute] string id)
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<Model.Class, Class>());

            try
            {
                var classEntity = config.CreateMapper().Map<Class>(request);

                if (await Task.Run(() => _classService.GetClasses().Any(x => x.id == classEntity.id)) == false)
                    return BadRequest(new { status = false, message = "Class id is not exist" });
                if (classEntity.startDate != null && !DateFormatRegex().IsMatch(classEntity.startDate))
                    return BadRequest(new { status = false, message = "Start date must be match YYYY-MM-DD format" });
                if (classEntity.endDate != null && classEntity.startDate != null && DateTime.Parse(classEntity.startDate) > DateTime.Parse(classEntity.endDate))
                    return BadRequest(new { status = false, message = "Start date must be less than end date" });
                if (await Task.Run(() => _classService.GetTeachers().Any(x => x != null && x.id == classEntity.teacherId)) == false)
                    return BadRequest(new { status = false, message = "Teacher id is not exist" });

                await Task.Run(() => _classService.UpdateClass(classEntity, id));
                return Ok(new { status = true, message = "Update class successfully", data = classEntity });
            }
            catch (Exception)
            {
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
        ///     DELETE /api/v1/class/deleteClass/string
        ///     {
        ///         "id": "string"
        ///     }
        /// </remarks>
        /// <response code="200">Xóa lớp học thành công</response>
        /// <response code="400">Lỗi dữ liệu đầu vào</response>
        /// <response code="500">Lỗi server</response>
        [HttpDelete("deleteClass/{id}")]
        public async Task<IActionResult> DeleteClass([FromRoute] string id)
        {
            try
            {
                if (await Task.Run(() => _classService.GetClasses().Any(x => x.id == id)) == false)
                    return BadRequest(new { status = false, message = "Class id is not exist" });

                await Task.Run(() => _classService.DeleteClass(id));
                return Ok(new { status = true, message = "Delete class successfully" });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { status = false, message = "An error occurred while processing your request" });
            }
        }
    }
}