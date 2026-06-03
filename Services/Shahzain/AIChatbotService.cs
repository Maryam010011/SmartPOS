using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using SmartPOS.Shared.Common;
using SmartPOS.Shared.DTOs.Sales;
using SmartPOS.Shared.Interfaces;
using SmartPOS.Web.Data;

namespace SmartPOS.Web.Services.Shahzain
{
    /// <summary>
    /// Service for AI-powered insights, sales analysis, and demand forecasting.
    /// Integrates with OpenAI's Chat Completions API to provide intelligent
    /// business analytics for the SmartPOS system.
    /// </summary>
    public class AIChatbotService : IAIChatbotService
    {
        private readonly HttpClient _httpClient;
        private readonly IDbContextFactory<AppDbContext> _factory;
        private readonly string _apiKey;
        private readonly string _model;
        private readonly string _baseUrl;

        /// <summary>
        /// Initializes a new instance of the <see cref="AIChatbotService"/> class.
        /// </summary>
        /// <param name="httpClient">The HTTP client for making API requests.</param>
        /// <param name="factory">The database context factory for querying sales data.</param>
        /// <param name="configuration">The application configuration for API key and model settings.</param>
        public AIChatbotService(HttpClient httpClient, IDbContextFactory<AppDbContext> factory, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _factory = factory;
            _apiKey = configuration["AIService:ApiKey"] ?? string.Empty;
            _model = configuration["AIService:Model"] ?? "gpt-4o-mini";
            _baseUrl = configuration["AIService:BaseUrl"] ?? "https://api.openai.com/v1";
        }

        /// <summary>
        /// Sends a user query to the AI API along with current sales context
        /// and returns an intelligent business insight.
        /// </summary>
        /// <param name="query">The user's natural-language question about business data.</param>
        /// <returns>An ApiResponse containing the AI-generated insight as a string.</returns>
        public async Task<ApiResponse<string>> GetInsight(string query)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(query))
                    return ApiResponse<string>.Fail("Query cannot be empty.");

                // Build sales context from database
                var salesContext = await BuildSalesContextAsync();

                var systemPrompt = $"""
                    You are SmartPOS AI Assistant — an intelligent business analytics assistant
                    for a bakery point-of-sale system. You help managers and owners understand
                    their sales data, identify trends, and make data-driven decisions.

                    Here is the current business context:
                    {salesContext}

                    Provide concise, actionable insights. Use numbers and percentages when possible.
                    Format your response in a clear, readable way.
                    """;

                var response = await SendChatRequestAsync(systemPrompt, query);
                return ApiResponse<string>.Ok(response, "Insight generated successfully.");
            }
            catch (HttpRequestException ex)
            {
                return ApiResponse<string>.Fail($"AI API connection error: {ex.Message}");
            }
            catch (Exception ex)
            {
                return ApiResponse<string>.Fail($"Error generating insight: {ex.Message}");
            }
        }

        /// <summary>
        /// Sends a summary of filtered sales data to the AI for analysis
        /// and returns structured analytics with chart data.
        /// </summary>
        /// <param name="filter">The sales filter criteria (date range, period, etc.).</param>
        /// <returns>An ApiResponse containing a SaleAnalyticsDto with AI-enhanced analysis.</returns>
        public async Task<ApiResponse<SaleAnalyticsDto>> AnalyzeSales(SaleFilterDto filter)
        {
            try
            {
                // Query sales from database based on filter
                using var context = _factory.CreateDbContext();
                var salesQuery = context.Sales
                    .Include(s => s.SaleItems)
                    .ThenInclude(si => si.Product)
                    .AsQueryable();

                if (filter.From.HasValue)
                    salesQuery = salesQuery.Where(s => s.SaleDate >= filter.From.Value);
                if (filter.To.HasValue)
                    salesQuery = salesQuery.Where(s => s.SaleDate <= filter.To.Value);
                if (filter.UserId.HasValue)
                    salesQuery = salesQuery.Where(s => s.UserId == filter.UserId.Value);
                if (filter.SaleType.HasValue)
                    salesQuery = salesQuery.Where(s => s.SaleType == filter.SaleType.Value);
                if (filter.Status.HasValue)
                    salesQuery = salesQuery.Where(s => s.Status == filter.Status.Value);

                var sales = await salesQuery.ToListAsync();

                if (!sales.Any())
                {
                    return ApiResponse<SaleAnalyticsDto>.Ok(new SaleAnalyticsDto
                    {
                        TotalRevenue = 0,
                        TotalTransactions = 0,
                        AverageOrderValue = 0,
                        ChartData = new List<SaleChartPointDto>()
                    }, "No sales found for the given filter.");
                }

                // Calculate base analytics
                var totalRevenue = sales.Sum(s => s.TotalAmount);
                var totalTransactions = sales.Count;
                var avgOrderValue = totalRevenue / totalTransactions;

                // Build chart data based on period
                var chartData = BuildChartData(sales, filter.Period);

                // Build a data summary for the AI to analyze
                var dataSummary = $"""
                    Sales Data Summary:
                    - Period: {filter.From?.ToString("yyyy-MM-dd") ?? "All time"} to {filter.To?.ToString("yyyy-MM-dd") ?? "Present"}
                    - Total Revenue: {totalRevenue:C}
                    - Total Transactions: {totalTransactions}
                    - Average Order Value: {avgOrderValue:C}
                    - Top Products: {GetTopProductsSummary(sales)}
                    - Sales by Type: {GetSalesByTypeSummary(sales)}
                    """;

                var systemPrompt = """
                    You are a sales analytics AI for a bakery POS system.
                    Analyze the provided sales data and return ONLY a valid JSON object
                    with no additional text, markdown, or explanation.
                    
                    The JSON must have this exact structure:
                    {
                        "TotalRevenue": <number>,
                        "TotalTransactions": <number>,
                        "AverageOrderValue": <number>,
                        "ChartData": [{"Label": "<string>", "Revenue": <number>, "Transactions": <number>}]
                    }
                    
                    Use the actual data values provided. Ensure ChartData matches the period grouping.
                    """;

                var aiResponse = await SendChatRequestAsync(systemPrompt, dataSummary);

                // Try to parse AI response; fall back to computed values
                var analytics = TryParseAnalytics(aiResponse) ?? new SaleAnalyticsDto
                {
                    TotalRevenue = totalRevenue,
                    TotalTransactions = totalTransactions,
                    AverageOrderValue = avgOrderValue,
                    ChartData = chartData
                };

                return ApiResponse<SaleAnalyticsDto>.Ok(analytics, "Sales analysis completed.");
            }
            catch (HttpRequestException ex)
            {
                return ApiResponse<SaleAnalyticsDto>.Fail($"AI API connection error: {ex.Message}");
            }
            catch (Exception ex)
            {
                return ApiResponse<SaleAnalyticsDto>.Fail($"Error analyzing sales: {ex.Message}");
            }
        }

        /// <summary>
        /// Sends a product's sales history to the AI and returns a
        /// natural-language demand forecast for that product.
        /// </summary>
        /// <param name="productId">The ID of the product to forecast demand for.</param>
        /// <returns>An ApiResponse containing the AI-generated demand forecast string.</returns>
        public async Task<ApiResponse<string>> ForecastDemand(int productId)
        {
            try
            {
                using var context = _factory.CreateDbContext();
                var product = await context.Products.FindAsync(productId);
                if (product == null)
                    return ApiResponse<string>.Fail("Product not found.");

                // Get sales history for this product
                var salesHistory = await context.SaleItems
                    .Include(si => si.Sale)
                    .Include(si => si.Product)
                    .Where(si => si.ProductId == productId && si.Sale.Status == Shared.Enums.SaleStatus.Completed)
                    .OrderBy(si => si.Sale.SaleDate)
                    .ToListAsync();

                if (!salesHistory.Any())
                    return ApiResponse<string>.Ok(
                        $"No sales history available for '{product.Name}'. Unable to generate a forecast without historical data.",
                        "No sales data.");

                // Build weekly sales summary
                var weeklySales = salesHistory
                    .GroupBy(si => new
                    {
                        Year = si.Sale.SaleDate.Year,
                        Week = System.Globalization.CultureInfo.CurrentCulture.Calendar
                            .GetWeekOfYear(si.Sale.SaleDate, System.Globalization.CalendarWeekRule.FirstDay, DayOfWeek.Monday)
                    })
                    .Select(g => new
                    {
                        g.Key.Year,
                        g.Key.Week,
                        TotalQty = g.Sum(si => si.Quantity),
                        TotalRevenue = g.Sum(si => si.LineTotal)
                    })
                    .OrderBy(x => x.Year)
                    .ThenBy(x => x.Week)
                    .ToList();

                var historySummary = string.Join("\n",
                    weeklySales.Select(w => $"  Year {w.Year}, Week {w.Week}: {w.TotalQty} units sold, Revenue: {w.TotalRevenue:C}"));

                var totalUnitsSold = salesHistory.Sum(si => si.Quantity);
                var totalRevenue = salesHistory.Sum(si => si.LineTotal);
                var avgWeeklyQty = weeklySales.Any() ? weeklySales.Average(w => w.TotalQty) : 0;

                var dataSummary = $"""
                    Product: {product.Name} (SKU: {product.SKU})
                    Price: {product.Price:C}
                    Cost Price: {product.CostPrice:C}
                    Total Units Sold (all time): {totalUnitsSold}
                    Total Revenue (all time): {totalRevenue:C}
                    Average Weekly Sales: {avgWeeklyQty:F1} units
                    Number of Weeks with Sales Data: {weeklySales.Count}

                    Weekly Sales History:
                    {historySummary}
                    """;

                var systemPrompt = """
                    You are a demand forecasting AI for a bakery POS system.
                    Based on the product's sales history, provide a demand forecast that includes:
                    1. Predicted demand for the next 4 weeks (units per week)
                    2. Trend analysis (increasing, decreasing, stable, seasonal)
                    3. Recommended stock levels
                    4. Any notable patterns (e.g., weekend spikes, seasonal trends)
                    5. Confidence level of the forecast (Low/Medium/High)

                    Keep the response concise, practical, and actionable for a bakery manager.
                    Use bullet points for clarity.
                    """;

                var forecast = await SendChatRequestAsync(systemPrompt, dataSummary);
                return ApiResponse<string>.Ok(forecast, "Demand forecast generated successfully.");
            }
            catch (HttpRequestException ex)
            {
                return ApiResponse<string>.Fail($"AI API connection error: {ex.Message}");
            }
            catch (Exception ex)
            {
                return ApiResponse<string>.Fail($"Error forecasting demand: {ex.Message}");
            }
        }

        // ────────────────────────────────────────────────────────────
        //  Private Helper Methods
        // ────────────────────────────────────────────────────────────

        /// <summary>
        /// Sends a chat completion request to the OpenAI-compatible API.
        /// </summary>
        /// <param name="systemPrompt">The system-level instruction for the AI model.</param>
        /// <param name="userMessage">The user's message or data to process.</param>
        /// <returns>The AI model's response text.</returns>
        private async Task<string> SendChatRequestAsync(string systemPrompt, string userMessage)
        {
            if (string.IsNullOrWhiteSpace(_apiKey))
                throw new InvalidOperationException(
                    "AI API key is not configured. Please set 'AIService:ApiKey' in appsettings.json.");

            var requestBody = new
            {
                model = _model,
                messages = new[]
                {
                    new { role = "system", content = systemPrompt },
                    new { role = "user", content = userMessage }
                },
                max_tokens = 1024,
                temperature = 0.7
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _apiKey);

            var response = await _httpClient.PostAsync($"{_baseUrl}/chat/completions", content);
            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(responseJson);

            var messageContent = doc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            return messageContent ?? "No response from AI.";
        }

        /// <summary>
        /// Builds a summary of current sales context from the database
        /// for the AI to use when answering general queries.
        /// </summary>
        private async Task<string> BuildSalesContextAsync()
        {
            var today = DateTime.UtcNow.Date;
            var thirtyDaysAgo = today.AddDays(-30);
            var sevenDaysAgo = today.AddDays(-7);

            using var context = _factory.CreateDbContext();
            var recentSales = await context.Sales
                .Where(s => s.SaleDate >= thirtyDaysAgo && s.Status == Shared.Enums.SaleStatus.Completed)
                .ToListAsync();

            var weekSales = recentSales.Where(s => s.SaleDate >= sevenDaysAgo).ToList();

            var totalProducts = await context.Products.CountAsync(p => p.IsActive);
            var totalCategories = await context.Categories.CountAsync();

            return $"""
                Business Overview (Last 30 Days):
                - Total Revenue: {recentSales.Sum(s => s.TotalAmount):C}
                - Total Transactions: {recentSales.Count}
                - Average Order Value: {(recentSales.Any() ? recentSales.Average(s => s.TotalAmount) : 0):C}
                - This Week Revenue: {weekSales.Sum(s => s.TotalAmount):C}
                - This Week Transactions: {weekSales.Count}
                - Active Products: {totalProducts}
                - Categories: {totalCategories}
                """;
        }

        /// <summary>
        /// Builds chart data points grouped by the specified period (daily, weekly, or monthly).
        /// </summary>
        private static List<SaleChartPointDto> BuildChartData(List<Models.Sale> sales, string period)
        {
            return period?.ToLower() switch
            {
                "weekly" => sales
                    .GroupBy(s => new
                    {
                        Year = s.SaleDate.Year,
                        Week = System.Globalization.CultureInfo.CurrentCulture.Calendar
                            .GetWeekOfYear(s.SaleDate, System.Globalization.CalendarWeekRule.FirstDay, DayOfWeek.Monday)
                    })
                    .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Week)
                    .Select(g => new SaleChartPointDto
                    {
                        Label = $"W{g.Key.Week} {g.Key.Year}",
                        Revenue = g.Sum(s => s.TotalAmount),
                        Transactions = g.Count()
                    })
                    .ToList(),

                "monthly" => sales
                    .GroupBy(s => new { s.SaleDate.Year, s.SaleDate.Month })
                    .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
                    .Select(g => new SaleChartPointDto
                    {
                        Label = $"{g.Key.Year}-{g.Key.Month:D2}",
                        Revenue = g.Sum(s => s.TotalAmount),
                        Transactions = g.Count()
                    })
                    .ToList(),

                _ => sales // Default: daily
                    .GroupBy(s => s.SaleDate.Date)
                    .OrderBy(g => g.Key)
                    .Select(g => new SaleChartPointDto
                    {
                        Label = g.Key.ToString("yyyy-MM-dd"),
                        Revenue = g.Sum(s => s.TotalAmount),
                        Transactions = g.Count()
                    })
                    .ToList()
            };
        }

        /// <summary>
        /// Returns a summary string of the top 5 selling products from the given sales.
        /// </summary>
        private static string GetTopProductsSummary(List<Models.Sale> sales)
        {
            var topProducts = sales
                .SelectMany(s => s.SaleItems)
                .GroupBy(si => si.Product?.Name ?? "Unknown")
                .OrderByDescending(g => g.Sum(si => si.Quantity))
                .Take(5)
                .Select(g => $"{g.Key} ({g.Sum(si => si.Quantity)} units)");

            return string.Join(", ", topProducts);
        }

        /// <summary>
        /// Returns a summary string of sales grouped by sale type.
        /// </summary>
        private static string GetSalesByTypeSummary(List<Models.Sale> sales)
        {
            var byType = sales
                .GroupBy(s => s.SaleType)
                .Select(g => $"{g.Key}: {g.Count()} transactions ({g.Sum(s => s.TotalAmount):C})");

            return string.Join(", ", byType);
        }

        /// <summary>
        /// Attempts to parse the AI response string into a SaleAnalyticsDto.
        /// Returns null if parsing fails.
        /// </summary>
        private static SaleAnalyticsDto? TryParseAnalytics(string aiResponse)
        {
            try
            {
                // Strip markdown code fences if present
                var json = aiResponse.Trim();
                if (json.StartsWith("```"))
                {
                    var firstNewline = json.IndexOf('\n');
                    var lastFence = json.LastIndexOf("```");
                    if (firstNewline > 0 && lastFence > firstNewline)
                        json = json[(firstNewline + 1)..lastFence].Trim();
                }

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    NumberHandling = JsonNumberHandling.AllowReadingFromString
                };

                return JsonSerializer.Deserialize<SaleAnalyticsDto>(json, options);
            }
            catch
            {
                return null;
            }
        }
    }
}
