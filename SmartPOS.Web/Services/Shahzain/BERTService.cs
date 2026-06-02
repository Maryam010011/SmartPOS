using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using SmartPOS.Shared.Common;
using SmartPOS.Shared.DTOs.Reviews;
using SmartPOS.Shared.Interfaces;
using SmartPOS.Web.Data;

namespace SmartPOS.Web.Services.Shahzain
{
    /// <summary>
    /// Service for sentiment analysis on customer reviews using a pre-trained
    /// BERT model via the HuggingFace Inference API. Provides real-time
    /// sentiment classification and aggregated sentiment statistics for
    /// admin dashboard charts.
    /// </summary>
    public class BERTService : IBERTService
    {
        private readonly HttpClient _httpClient;
        private readonly IDbContextFactory<AppDbContext> _factory;
        private readonly string _apiKey;
        private readonly string _modelId;
        private readonly string _baseUrl;

        /// <summary>
        /// Initializes a new instance of the <see cref="BERTService"/> class.
        /// </summary>
        /// <param name="httpClient">The HTTP client for making HuggingFace API requests.</param>
        /// <param name="factory">The database context factory for querying reviews.</param>
        /// <param name="configuration">The application configuration for HuggingFace API credentials.</param>
        public BERTService(HttpClient httpClient, IDbContextFactory<AppDbContext> factory, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _factory = factory;
            _apiKey = configuration["BERTService:ApiKey"] ?? string.Empty;
            _modelId = configuration["BERTService:ModelId"] ?? "nlptown/bert-base-multilingual-uncased-sentiment";
            _baseUrl = configuration["BERTService:BaseUrl"] ?? "https://api-inference.huggingface.co";
        }

        /// <summary>
        /// Sends review text to the HuggingFace BERT Inference API for sentiment
        /// classification. Returns a sentiment label: Positive, Negative, or Neutral.
        /// </summary>
        /// <param name="reviewText">The customer review text to analyze.</param>
        /// <returns>
        /// An ApiResponse containing the sentiment label ("Positive", "Negative", or "Neutral").
        /// </returns>
        public async Task<ApiResponse<string>> AnalyzeSentiment(string reviewText)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(reviewText))
                    return ApiResponse<string>.Fail("Review text cannot be empty.");

                if (string.IsNullOrWhiteSpace(_apiKey))
                    return ApiResponse<string>.Fail(
                        "HuggingFace API key is not configured. Set 'BERTService:ApiKey' in appsettings.json.");

                // Build request payload for HuggingFace Inference API
                var requestBody = new { inputs = reviewText };
                var json = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", _apiKey);

                var response = await _httpClient.PostAsync(
                    $"{_baseUrl}/models/{_modelId}", content);

                // Handle model loading (HuggingFace returns 503 while loading)
                if (response.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable)
                {
                    return ApiResponse<string>.Fail(
                        "BERT model is currently loading. Please try again in a few seconds.");
                }

                if (!response.IsSuccessStatusCode)
                {
                    var errorBody = await response.Content.ReadAsStringAsync();
                    return ApiResponse<string>.Fail(
                        $"HuggingFace API returned {response.StatusCode}: {errorBody}");
                }

                var responseJson = await response.Content.ReadAsStringAsync();
                var sentiment = ParseSentimentResponse(responseJson);

                return ApiResponse<string>.Ok(sentiment, "Sentiment analysis completed.");
            }
            catch (HttpRequestException ex)
            {
                return ApiResponse<string>.Fail($"HuggingFace API connection error: {ex.Message}");
            }
            catch (Exception ex)
            {
                return ApiResponse<string>.Fail($"Error analyzing sentiment: {ex.Message}");
            }
        }

        /// <summary>
        /// Queries all reviews from the database (optionally filtered by product),
        /// groups them by product, and returns aggregated sentiment counts and
        /// average ratings for admin dashboard charts.
        /// </summary>
        /// <param name="productId">
        /// Optional product ID to filter reviews. If null, returns stats for all products.
        /// </param>
        /// <returns>
        /// An ApiResponse containing a list of SentimentStatDto with per-product
        /// sentiment breakdowns (Positive, Neutral, Negative counts and average rating).
        /// </returns>
        public async Task<ApiResponse<List<SentimentStatDto>>> GetSentimentStats(int? productId = null)
        {
            try
            {
                using var context = _factory.CreateDbContext();
                var query = context.Reviews
                    .Include(r => r.Product)
                    .AsQueryable();

                if (productId.HasValue)
                    query = query.Where(r => r.ProductId == productId.Value);

                var reviews = await query.ToListAsync();

                if (!reviews.Any())
                {
                    return ApiResponse<List<SentimentStatDto>>.Ok(
                        new List<SentimentStatDto>(), "No reviews found.");
                }

                var stats = reviews
                    .GroupBy(r => new { r.ProductId, ProductName = r.Product?.Name ?? "Unknown" })
                    .Select(g => new SentimentStatDto
                    {
                        ProductName = g.Key.ProductName,
                        TotalReviews = g.Count(),
                        Positive = g.Count(r => string.Equals(r.Sentiment, "Positive", StringComparison.OrdinalIgnoreCase)),
                        Neutral = g.Count(r => string.Equals(r.Sentiment, "Neutral", StringComparison.OrdinalIgnoreCase)),
                        Negative = g.Count(r => string.Equals(r.Sentiment, "Negative", StringComparison.OrdinalIgnoreCase)),
                        AverageRating = Math.Round(g.Average(r => r.Rating), 2)
                    })
                    .OrderByDescending(s => s.TotalReviews)
                    .ToList();

                return ApiResponse<List<SentimentStatDto>>.Ok(stats, "Sentiment statistics retrieved successfully.");
            }
            catch (Exception ex)
            {
                return ApiResponse<List<SentimentStatDto>>.Fail($"Error retrieving sentiment stats: {ex.Message}");
            }
        }

        // ────────────────────────────────────────────────────────────
        //  Private Helper Methods
        // ────────────────────────────────────────────────────────────

        /// <summary>
        /// Parses the HuggingFace Inference API response and maps the
        /// top-scoring label to a Positive/Negative/Neutral sentiment.
        /// </summary>
        /// <remarks>
        /// The nlptown/bert-base-multilingual-uncased-sentiment model returns
        /// labels like "1 star" through "5 stars". This method maps:
        /// - 4-5 stars → Positive
        /// - 3 stars → Neutral
        /// - 1-2 stars → Negative
        ///
        /// For other models that return POSITIVE/NEGATIVE/NEUTRAL directly,
        /// the labels are normalized to title case.
        /// </remarks>
        /// <param name="responseJson">The raw JSON response from the HuggingFace API.</param>
        /// <returns>A normalized sentiment label string.</returns>
        private static string ParseSentimentResponse(string responseJson)
        {
            using var doc = JsonDocument.Parse(responseJson);
            var root = doc.RootElement;

            // HuggingFace returns [[{label, score}, ...]] (nested array)
            JsonElement predictions;
            if (root.ValueKind == JsonValueKind.Array && root.GetArrayLength() > 0)
            {
                var firstElement = root[0];
                predictions = firstElement.ValueKind == JsonValueKind.Array
                    ? firstElement
                    : root;
            }
            else
            {
                return "Neutral";
            }

            // Find the label with the highest score
            string topLabel = "Neutral";
            double topScore = 0;

            foreach (var prediction in predictions.EnumerateArray())
            {
                var label = prediction.GetProperty("label").GetString() ?? string.Empty;
                var score = prediction.GetProperty("score").GetDouble();

                if (score > topScore)
                {
                    topScore = score;
                    topLabel = label;
                }
            }

            // Map star-based labels to sentiment categories
            return MapLabelToSentiment(topLabel);
        }

        /// <summary>
        /// Maps a model-specific label to a standardized sentiment category.
        /// Handles both star-rating labels (e.g., "5 stars") and direct
        /// sentiment labels (e.g., "POSITIVE").
        /// </summary>
        /// <param name="label">The raw label from the BERT model.</param>
        /// <returns>A normalized sentiment string: "Positive", "Negative", or "Neutral".</returns>
        private static string MapLabelToSentiment(string label)
        {
            var normalized = label.ToLower().Trim();

            // Handle star-based labels (nlptown model)
            if (normalized.Contains("5 star") || normalized.Contains("4 star"))
                return "Positive";
            if (normalized.Contains("3 star"))
                return "Neutral";
            if (normalized.Contains("1 star") || normalized.Contains("2 star"))
                return "Negative";

            // Handle direct sentiment labels (other models)
            if (normalized.Contains("positive") || normalized == "pos" || normalized == "label_2")
                return "Positive";
            if (normalized.Contains("negative") || normalized == "neg" || normalized == "label_0")
                return "Negative";
            if (normalized.Contains("neutral") || normalized == "label_1")
                return "Neutral";

            return "Neutral";
        }
    }
}
