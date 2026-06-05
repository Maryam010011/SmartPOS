using Microsoft.AspNetCore.Mvc;
using SmartPOS.Shared.DTOs.Categories;
using SmartPOS.Shared.Interfaces;

namespace SmartPOS.Web.Controllers
{
    /// <summary>
    /// API controller for managing categories in the SmartPOS system.
    /// Provides endpoints for CRUD operations and hierarchical tree retrieval.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        /// <summary>
        /// Initializes a new instance of the <see cref="CategoryController"/> class.
        /// </summary>
        /// <param name="categoryService">The category service instance injected via DI.</param>
        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        /// <summary>
        /// Retrieves all categories as a flat list.
        /// </summary>
        /// <returns>An ApiResponse containing a list of all categories.</returns>
        /// <response code="200">Returns the list of categories.</response>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _categoryService.GetAll();
            return Ok(result);
        }

        /// <summary>
        /// Retrieves all categories as a nested tree structure.
        /// Parent categories contain their subcategories in the SubCategories property.
        /// </summary>
        /// <returns>An ApiResponse containing the category tree.</returns>
        /// <response code="200">Returns the nested category tree.</response>
        [HttpGet("tree")]
        public async Task<IActionResult> GetTree()
        {
            var result = await _categoryService.GetTree();
            return Ok(result);
        }

        /// <summary>
        /// Retrieves a specific category by its unique identifier.
        /// </summary>
        /// <param name="id">The category ID.</param>
        /// <returns>An ApiResponse containing the requested category.</returns>
        /// <response code="200">Returns the category.</response>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _categoryService.GetById(id);
            return Ok(result);
        }

        /// <summary>
        /// Creates a new category.
        /// </summary>
        /// <param name="dto">The category creation data. Set ParentCategoryId for subcategories.</param>
        /// <returns>An ApiResponse containing the newly created category.</returns>
        /// <response code="200">Returns the created category.</response>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCategoryDto dto)
        {
            var result = await _categoryService.Create(dto);
            return Ok(result);
        }

        /// <summary>
        /// Updates an existing category.
        /// </summary>
        /// <param name="dto">The category update data including the category ID.</param>
        /// <returns>An ApiResponse containing the updated category.</returns>
        /// <response code="200">Returns the updated category.</response>
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateCategoryDto dto)
        {
            var result = await _categoryService.Update(dto);
            return Ok(result);
        }

        /// <summary>
        /// Deletes a category by its unique identifier.
        /// </summary>
        /// <param name="id">The category ID to delete.</param>
        /// <returns>An ApiResponse indicating success or failure.</returns>
        /// <response code="200">Returns the deletion result.</response>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _categoryService.Delete(id);
            return Ok(result);
        }
    }
}
