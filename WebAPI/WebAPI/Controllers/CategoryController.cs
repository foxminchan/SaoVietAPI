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
    /// Quản lý danh mục
    /// </summary>
    [Route("api/v1/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ILogger<CategoryController> _logger;
        private readonly IMapper _mapper;
        private readonly CategoryService _studentService;

        /// <inheritdoc />
        public CategoryController(ILogger<CategoryController> logger, CategoryService studentService)
        {
            _logger = logger;
            _mapper = new MapperConfiguration(cfg => cfg.CreateMap<Models.Category, Domain.Entities.Category>()).CreateMapper();
            _studentService = studentService;
        }

        private static bool IsValidCategory(Models.Category category, out string message)
        {
            if (string.IsNullOrEmpty(category.id))
            {
                message = "Category id is required";
                return false;
            }

            if (string.IsNullOrEmpty(category.name))
            {
                message = "Category name is required";
                return false;
            }

            message = string.Empty;
            return true;
        }

        /// <summary>
        /// Lấy danh sách danh mục
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/v1/Category/GetCategories
        /// </remarks>
        /// <response code="200">Trả về danh sách danh mục</response>
        /// <response code="204">Không có danh mục nào</response>
        /// <response code="429">Request quá nhiều</response>
        /// <response code="500">Lỗi server</response>
        [HttpGet("getCategories")]
        public async Task<IActionResult> GetCategories()
        {
            try
            {
                var categories = await Task.Run(_studentService.GetCategories);
                return categories.Any()
                    ? Ok(new { status = true, message = "Get data successfully", data = categories })
                    : NoContent();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while getting all categories");
                return StatusCode(StatusCodes.Status500InternalServerError, new { status = false, message = "An error occurred while processing your request" });
            }
        }

        /// <summary>
        /// Tìm danh mục theo id
        /// </summary>
        /// <param name="id">Mã danh mục</param>
        /// <returns></returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/v1/Category/findById/string
        ///     {
        ///         "id": "string"
        ///     }
        /// </remarks>
        /// <response code="200">Trả về danh mục</response>
        /// <response code="404">Không tìm thấy danh mục</response>
        /// <response code="429">Request quá nhiều</response>
        /// <response code="500">Lỗi server</response>
        [HttpGet("findById/{id}")]
        public async Task<IActionResult> GetCategoryById(string? id)
        {
            try
            {
                var category = await Task.Run(() => _studentService.GetCategoryById(id));
                return category != null
                    ? Ok(new { status = true, message = "Get data successfully", data = category })
                    : NotFound(new { status = false, message = "Category not found" });
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while getting category by id");
                return StatusCode(StatusCodes.Status500InternalServerError, new { status = false, message = "An error occurred while processing your request" });
            }
        }

        /// <summary>
        /// Thêm danh mục
        /// </summary>
        /// <param name="category">Đối tượng danh mục</param>
        /// <returns></returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/v1/Category/addCategory
        ///     {
        ///         "id": "string",
        ///         "name": "string"
        ///     }
        /// </remarks>
        /// <response code="200">Thêm danh mục thành công</response>
        /// <response code="400">Lỗi dữ liệu đầu vào</response>
        /// <response code="429">Request quá nhiều</response>
        /// <response code="500">Lỗi server</response>
        [HttpPost("addCategory")]
        public async Task<IActionResult> AddCategory([FromBody] Models.Category category)
        {
            if (!IsValidCategory(category, out var message))
                return BadRequest(new { status = false, message });
            
            try
            {
                if (!await Task.Run(() => category.id != null && _studentService.CategoryExists(category.id)))
                    return BadRequest(new { status = false, message = "Category id already exists" });
                var categoryEntity = _mapper.Map<Domain.Entities.Category>(category);
                await _studentService.AddCategory(categoryEntity);
                return Ok(new { status = true, message = "Add category successfully" });
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while adding category");
                return StatusCode(StatusCodes.Status500InternalServerError, new { status = false, message = "An error occurred while processing your request" });
            }
        }

        /// <summary>
        /// Cập nhật danh mục
        /// </summary>
        /// <param name="category">Đối tượng danh mục</param>
        /// <param name="id">Mã danh mục</param>
        /// <returns></returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT /api/v1/Category/updateCategory/string
        ///     {
        ///         "id": "string",
        ///         "name": "string"
        ///     }
        /// </remarks>
        /// <response code="200">Cập nhật danh mục thành công</response>
        /// <response code="400">Lỗi dữ liệu đầu vào</response>
        /// <response code="429">Request quá nhiều</response>
        /// <response code="500">Lỗi server</response>
        [HttpPut("updateCategory/{id}")]
        public async Task<IActionResult> UpdateCategory([FromBody] Models.Category category, [FromRoute] string? id)
        {
            if (!IsValidCategory(category, out var message))
                return BadRequest(new { status = false, message });

            try
            {
                if (await Task.Run(() => category.id != null && _studentService.CategoryExists(category.id)))
                    return BadRequest(new { status = false, message = "Category id is not exists" });
                var categoryEntity = _mapper.Map<Domain.Entities.Category>(category);
                if (id != null) await _studentService.UpdateCategory(categoryEntity, id);
                return Ok(new { status = true, message = "Update category successfully" });
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while updating category");
                return StatusCode(StatusCodes.Status500InternalServerError, new { status = false, message = "An error occurred while processing your request" });
            }
        }

        /// <summary>
        /// Xóa danh mục
        /// </summary>
        /// <param name="id">Mã danh mục</param>
        /// <returns></returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     DELETE /api/v1/Category/deleteCategory/string
        ///     {
        ///         "id": "string"
        ///     }
        /// </remarks>
        /// <response code="200">Xóa danh mục thành công</response>
        /// <response code="400">Lỗi dữ liệu đầu vào</response>
        /// <response code="429">Request quá nhiều</response>
        /// <response code="500">Lỗi server</response>
        [HttpDelete("deleteCategory/{id}")]
        public async Task<IActionResult> DeleteCategory([FromRoute] string? id)
        {
            try
            {
                if (await Task.Run(() => id != null && _studentService.CategoryExists(id)))
                    return BadRequest(new { status = false, message = "Category id is not exists" });
                if (id != null) await _studentService.DeleteCategory(id);
                return Ok(new { status = true, message = "Delete category successfully" });
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while deleting category");
                return StatusCode(StatusCodes.Status500InternalServerError, new { status = false, message = "An error occurred while processing your request" });
            }
        }
    }
}
