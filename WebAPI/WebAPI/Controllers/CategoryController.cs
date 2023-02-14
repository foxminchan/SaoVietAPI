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
    /// Quản lý danh mục
    /// </summary>
    [Authorize]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ILogger<CategoryController> _logger;
        private readonly IMapper _mapper;
        private readonly CategoryService _studentService;
        private readonly TransactionService _transactionService;

        /// <inheritdoc />
        public CategoryController(
            CategoryService studentService,
            TransactionService transactionService,
            ILogger<CategoryController> logger )
        {
            _logger = logger;
            _mapper = new MapperConfiguration(cfg => cfg.CreateMap<Models.Category, Domain.Entities.Category>()).CreateMapper();
            _studentService = studentService;
            _transactionService = transactionService;
        }

        private static bool IsValidCategory(Models.Category category, out string? message)
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

            message = null;
            return true;
        }

        /// <summary>
        /// Lấy danh sách danh mục
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/v1/Category
        /// </remarks>
        /// <response code="200">Trả về danh sách danh mục</response>
        /// <response code="204">Không có danh mục nào</response>
        /// <response code="429">Request quá nhiều</response>
        /// <response code="500">Lỗi server</response>
        [HttpGet]
        [AllowAnonymous]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any, VaryByQueryKeys = new[] { "categories" })]
        public ActionResult GetCategories()
        {
            try
            {
                var categories = _studentService.GetCategories().ToArray();
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
        ///     GET /api/v1/Category/string
        /// </remarks>
        /// <response code="200">Trả về danh mục</response>
        /// <response code="404">Không tìm thấy danh mục</response>
        /// <response code="429">Request quá nhiều</response>
        /// <response code="500">Lỗi server</response>
        [HttpGet("{id}")]
        [AllowAnonymous]
        public ActionResult GetCategoryById([FromRoute] string? id)
        {
            try
            {
                var category = _studentService.GetCategoryById(id);
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
        ///     POST /api/v1/Category
        ///     {
        ///         "id": "string",
        ///         "name": "string"
        ///     }
        /// </remarks>
        /// <response code="200">Thêm danh mục thành công</response>
        /// <response code="400">Lỗi dữ liệu đầu vào</response>
        /// <response code="401">Không có quyền</response>
        /// <response code="429">Request quá nhiều</response>
        /// <response code="500">Lỗi server</response>
        [HttpPost]
        [Authorize]
        public ActionResult AddCategory([FromBody] Models.Category category)
        {
            if(!IsValidCategory(category, out var message))
                return BadRequest(new { status = false, message });

            try
            {
                if (category.id != null && !_studentService.CategoryExists(category.id))
                    return BadRequest(new { status = false, message = "Category id already exists" });
                var categoryEntity = _mapper.Map<Domain.Entities.Category>(category);
                _transactionService.ExecuteTransaction(() => { _studentService.AddCategory(categoryEntity); });
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
        /// <returns></returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT /api/v1/Category
        ///     {
        ///         "id": "string",
        ///         "name": "string"
        ///     }
        /// </remarks>
        /// <response code="200">Cập nhật danh mục thành công</response>
        /// <response code="400">Lỗi dữ liệu đầu vào</response>
        /// <response code="401">Không có quyền</response>
        /// <response code="429">Request quá nhiều</response>
        /// <response code="500">Lỗi server</response>
        [HttpPut]
        [Authorize]
        public ActionResult UpdateCategory([FromBody] Models.Category category)
        {
            if (!IsValidCategory(category, out var message))
                return BadRequest(new { status = false, message });

            try
            {
                if (category.id != null && _studentService.CategoryExists(category.id))
                    return BadRequest(new { status = false, message = "Category id is not exists" });
                var categoryEntity = _mapper.Map<Domain.Entities.Category>(category);
                _transactionService.ExecuteTransaction(() => { _studentService.UpdateCategory(categoryEntity); });
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
        ///     DELETE /api/v1/Category/string
        /// </remarks>
        /// <response code="200">Xóa danh mục thành công</response>
        /// <response code="400">Lỗi dữ liệu đầu vào</response>
        /// <response code="401">Không có quyền</response>
        /// <response code="429">Request quá nhiều</response>
        /// <response code="500">Lỗi server</response>
        [HttpDelete("{id}")]
        [Authorize]
        public ActionResult DeleteCategory([FromRoute] string id)
        {
            try
            {
                if (_studentService.CategoryExists(id))
                    return BadRequest(new { status = false, message = "Category id is not exists" });
                _transactionService.ExecuteTransaction(() => { _studentService.DeleteCategory(id); });
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
