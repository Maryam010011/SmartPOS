using Microsoft.AspNetCore.Mvc;
using SmartPOS.Shared.DTOs.Products;
using SmartPOS.Shared.Interfaces;

namespace SmartPOS.Web.Controllers
{
    /// <summary>
    /// API controller for managing products in the SmartPOS system.
    /// Provides endpoints for CRUD operations, filtering, and search.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductController"/> class.
        /// </summary>
        /// <param name="productService">The product service instance injected via DI.</param>
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        /// <summary>
        /// Retrieves all products.
        /// </summary>
        /// <returns>An ApiResponse containing a list of all products.</returns>
        /// <response code="200">Returns the list of products.</response>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _productService.GetAll();
            return Ok(result);
        }

        /// <summary>
        /// Retrieves a specific product by its unique identifier.
        /// </summary>
        /// <param name="id">The product ID.</param>
        /// <returns>An ApiResponse containing the requested product.</returns>
        /// <response code="200">Returns the product.</response>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _productService.GetById(id);
            return Ok(result);
        }

        /// <summary>
        /// Retrieves all products belonging to a specific category.
        /// </summary>
        /// <param name="categoryId">The category ID to filter by.</param>
        /// <returns>An ApiResponse containing a list of products in the specified category.</returns>
        /// <response code="200">Returns the filtered list of products.</response>
        [HttpGet("by-category/{categoryId}")]
        public async Task<IActionResult> GetByCategory(int categoryId)
        {
            var result = await _productService.GetByCategory(categoryId);
            return Ok(result);
        }

        /// <summary>
        /// Retrieves all products supplied by a specific supplier.
        /// </summary>
        /// <param name="supplierId">The supplier ID to filter by.</param>
        /// <returns>An ApiResponse containing a list of products from the specified supplier.</returns>
        /// <response code="200">Returns the filtered list of products.</response>
        [HttpGet("by-supplier/{supplierId}")]
        public async Task<IActionResult> GetBySupplier(int supplierId)
        {
            var result = await _productService.GetBySupplier(supplierId);
            return Ok(result);
        }

        /// <summary>
        /// Creates a new product.
        /// </summary>
        /// <param name="dto">The product creation data.</param>
        /// <returns>An ApiResponse containing the newly created product.</returns>
        /// <response code="200">Returns the created product.</response>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProductDto dto)
        {
            var result = await _productService.Create(dto);
            return Ok(result);
        }

        /// <summary>
        /// Updates an existing product.
        /// </summary>
        /// <param name="dto">The product update data including the product ID.</param>
        /// <returns>An ApiResponse containing the updated product.</returns>
        /// <response code="200">Returns the updated product.</response>
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateProductDto dto)
        {
            var result = await _productService.Update(dto);
            return Ok(result);
        }

        /// <summary>
        /// Deletes a product by its unique identifier.
        /// </summary>
        /// <param name="id">The product ID to delete.</param>
        /// <returns>An ApiResponse indicating success or failure.</returns>
        /// <response code="200">Returns the deletion result.</response>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _productService.Delete(id);
            return Ok(result);
        }

        /// <summary>
        /// Toggles the active/inactive status of a product.
        /// </summary>
        /// <param name="id">The product ID to toggle.</param>
        /// <returns>An ApiResponse indicating the new active status.</returns>
        /// <response code="200">Returns the toggle result.</response>
        [HttpPatch("{id}/toggle-active")]
        public async Task<IActionResult> ToggleActive(int id)
        {
            var result = await _productService.ToggleActive(id);
            return Ok(result);
        }

        /// <summary>
        /// Searches for products by name or SKU keyword.
        /// </summary>
        /// <param name="keyword">The search keyword.</param>
        /// <returns>An ApiResponse containing a list of matching products.</returns>
        /// <response code="200">Returns the search results.</response>
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string keyword)
        {
            var result = await _productService.Search(keyword);
            return Ok(result);
        }
    }
}
