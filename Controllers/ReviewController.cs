using Microsoft.AspNetCore.Mvc;
using SmartPOS.Shared.DTOs.Reviews;
using SmartPOS.Shared.Interfaces;

namespace SmartPOS.Web.Controllers
{
    /// <summary>
    /// API controller for managing product reviews and ratings in the SmartPOS system.
    /// Provides endpoints for creating, retrieving, and deleting reviews.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReviewController"/> class.
        /// </summary>
        /// <param name="reviewService">The review service instance injected via DI.</param>
        public ReviewController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        /// <summary>
        /// Creates a new product review.
        /// </summary>
        /// <param name="dto">The review creation data including customer, product, rating, and comment.</param>
        /// <returns>An ApiResponse containing the newly created review with sentiment analysis.</returns>
        /// <response code="200">Returns the created review.</response>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateReviewDto dto)
        {
            var result = await _reviewService.Create(dto);
            return Ok(result);
        }

        /// <summary>
        /// Retrieves all reviews for a specific product.
        /// </summary>
        /// <param name="productId">The product ID to retrieve reviews for.</param>
        /// <returns>An ApiResponse containing a list of reviews for the specified product.</returns>
        /// <response code="200">Returns the list of reviews for the product.</response>
        [HttpGet("by-product/{productId}")]
        public async Task<IActionResult> GetByProduct(int productId)
        {
            var result = await _reviewService.GetByProduct(productId);
            return Ok(result);
        }

        /// <summary>
        /// Retrieves all reviews across all products.
        /// </summary>
        /// <returns>An ApiResponse containing a list of all reviews.</returns>
        /// <response code="200">Returns the list of all reviews.</response>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _reviewService.GetAll();
            return Ok(result);
        }

        /// <summary>
        /// Deletes a review by its unique identifier.
        /// </summary>
        /// <param name="id">The review ID to delete.</param>
        /// <returns>An ApiResponse indicating success or failure of the deletion.</returns>
        /// <response code="200">Returns the deletion result.</response>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _reviewService.Delete(id);
            return Ok(result);
        }
    }
}
