using Microsoft.EntityFrameworkCore;
using SmartPOS.Shared.Common;
using SmartPOS.Shared.DTOs.Reviews;
using SmartPOS.Shared.Interfaces;
using SmartPOS.Web.Data;
using SmartPOS.Web.Models;

namespace SmartPOS.Web.Services.Shahzain
{
    /// <summary>
    /// Service for managing customer reviews in the SmartPOS system.
    /// Integrates with IBERTService to perform sentiment analysis on
    /// review comments upon creation. Implements CRUD operations with
    /// robust error handling.
    /// </summary>
    public class ReviewService : IReviewService
    {
        private readonly AppDbContext _context;
        private readonly IBERTService _bertService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReviewService"/> class.
        /// </summary>
        /// <param name="context">The application database context.</param>
        /// <param name="bertService">The BERT sentiment analysis service.</param>
        public ReviewService(AppDbContext context, IBERTService bertService)
        {
            _context = context;
            _bertService = bertService;
        }

        /// <summary>
        /// Creates a new customer review and automatically analyzes its sentiment
        /// using the BERT service. The review's Sentiment and SentimentScore fields
        /// are populated based on the AI analysis result.
        /// </summary>
        /// <param name="dto">The review creation data transfer object.</param>
        /// <returns>An ApiResponse containing the created ReviewDto with sentiment data.</returns>
        public async Task<ApiResponse<ReviewDto>> Create(CreateReviewDto dto)
        {
            try
            {
                // Validate rating range
                if (dto.Rating < 1 || dto.Rating > 5)
                    return ApiResponse<ReviewDto>.Fail("Rating must be between 1 and 5.");

                // Validate product exists
                var product = await _context.Products.FindAsync(dto.ProductId);
                if (product == null)
                    return ApiResponse<ReviewDto>.Fail("Product not found.");

                var review = new Review
                {
                    CustomerId = dto.CustomerId,
                    ProductId = dto.ProductId,
                    Rating = dto.Rating,
                    Comment = dto.Comment,
                    CreatedAt = DateTime.UtcNow
                };

                // Analyze sentiment via BERT if comment is provided
                if (!string.IsNullOrWhiteSpace(dto.Comment))
                {
                    var sentimentResult = await _bertService.AnalyzeSentiment(dto.Comment);
                    if (sentimentResult.Success && sentimentResult.Data != null)
                    {
                        review.Sentiment = sentimentResult.Data;
                        review.SentimentScore = MapSentimentToScore(sentimentResult.Data);
                    }
                    else
                    {
                        // Fallback: derive sentiment from rating if BERT is unavailable
                        review.Sentiment = DeriveSentimentFromRating(dto.Rating);
                        review.SentimentScore = MapSentimentToScore(review.Sentiment);
                    }
                }
                else
                {
                    // No comment provided — derive sentiment from rating alone
                    review.Sentiment = DeriveSentimentFromRating(dto.Rating);
                    review.SentimentScore = MapSentimentToScore(review.Sentiment);
                }

                _context.Reviews.Add(review);
                await _context.SaveChangesAsync();

                // Reload with navigation properties for mapping
                var savedReview = await _context.Reviews
                    .Include(r => r.Product)
                    .FirstOrDefaultAsync(r => r.Id == review.Id);

                return ApiResponse<ReviewDto>.Ok(MapToDto(savedReview!), "Review created successfully.");
            }
            catch (Exception ex)
            {
                return ApiResponse<ReviewDto>.Fail($"Error creating review: {ex.Message}");
            }
        }

        /// <summary>
        /// Retrieves all reviews for a specific product, ordered by most recent first.
        /// </summary>
        /// <param name="productId">The product ID to get reviews for.</param>
        /// <returns>An ApiResponse containing a list of ReviewDto objects for the product.</returns>
        public async Task<ApiResponse<List<ReviewDto>>> GetByProduct(int productId)
        {
            try
            {
                var product = await _context.Products.FindAsync(productId);
                if (product == null)
                    return ApiResponse<List<ReviewDto>>.Fail("Product not found.");

                var reviews = await _context.Reviews
                    .Include(r => r.Product)
                    .Where(r => r.ProductId == productId)
                    .OrderByDescending(r => r.CreatedAt)
                    .ToListAsync();

                return ApiResponse<List<ReviewDto>>.Ok(reviews.Select(MapToDto).ToList());
            }
            catch (Exception ex)
            {
                return ApiResponse<List<ReviewDto>>.Fail($"Error retrieving reviews: {ex.Message}");
            }
        }

        /// <summary>
        /// Retrieves all reviews from the database, ordered by most recent first.
        /// </summary>
        /// <returns>An ApiResponse containing a list of all ReviewDto objects.</returns>
        public async Task<ApiResponse<List<ReviewDto>>> GetAll()
        {
            try
            {
                var reviews = await _context.Reviews
                    .Include(r => r.Product)
                    .OrderByDescending(r => r.CreatedAt)
                    .ToListAsync();

                return ApiResponse<List<ReviewDto>>.Ok(reviews.Select(MapToDto).ToList());
            }
            catch (Exception ex)
            {
                return ApiResponse<List<ReviewDto>>.Fail($"Error retrieving reviews: {ex.Message}");
            }
        }

        /// <summary>
        /// Deletes a review from the database by its ID.
        /// </summary>
        /// <param name="id">The review ID.</param>
        /// <returns>An ApiResponse indicating success or failure.</returns>
        public async Task<ApiResponse> Delete(int id)
        {
            try
            {
                var review = await _context.Reviews.FindAsync(id);
                if (review == null)
                    return ApiResponse.Fail("Review not found.");

                _context.Reviews.Remove(review);
                await _context.SaveChangesAsync();

                return ApiResponse.Ok("Review deleted successfully.");
            }
            catch (Exception ex)
            {
                return ApiResponse.Fail($"Error deleting review: {ex.Message}");
            }
        }

        // ────────────────────────────────────────────────────────────
        //  Private Helper Methods
        // ────────────────────────────────────────────────────────────

        /// <summary>
        /// Maps a Review entity to a ReviewDto.
        /// </summary>
        /// <param name="review">The Review entity.</param>
        /// <returns>A ReviewDto populated from the entity.</returns>
        private static ReviewDto MapToDto(Review review)
        {
            return new ReviewDto
            {
                Id = review.Id,
                CustomerId = review.CustomerId,
                CustomerName = string.Empty, // TODO: Populate when Customer model is available
                ProductId = review.ProductId,
                ProductName = review.Product?.Name ?? "Unknown",
                Rating = review.Rating,
                Comment = review.Comment,
                Sentiment = review.Sentiment,
                SentimentScore = review.SentimentScore.HasValue ? (float)review.SentimentScore.Value : 0f,
                CreatedAt = review.CreatedAt
            };
        }

        /// <summary>
        /// Derives a sentiment label from a numeric rating as a fallback
        /// when BERT analysis is unavailable or no comment is provided.
        /// </summary>
        /// <param name="rating">The review rating (1-5).</param>
        /// <returns>"Positive", "Neutral", or "Negative".</returns>
        private static string DeriveSentimentFromRating(int rating)
        {
            return rating switch
            {
                >= 4 => "Positive",
                3 => "Neutral",
                _ => "Negative"
            };
        }

        /// <summary>
        /// Maps a sentiment label to a numeric score for storage and sorting.
        /// </summary>
        /// <param name="sentiment">The sentiment label.</param>
        /// <returns>A float score: 1.0 for Positive, 0.5 for Neutral, 0.0 for Negative.</returns>
        private static float MapSentimentToScore(string sentiment)
        {
            return sentiment switch
            {
                "Positive" => 1.0f,
                "Neutral" => 0.5f,
                "Negative" => 0.0f,
                _ => 0.5f
            };
        }
    }
}
