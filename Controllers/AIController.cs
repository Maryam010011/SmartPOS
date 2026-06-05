using Microsoft.AspNetCore.Mvc;
using SmartPOS.Shared.DTOs.Sales;
using SmartPOS.Shared.Interfaces;

namespace SmartPOS.Web.Controllers
{
    /// <summary>
    /// API controller for AI-powered chatbot and analytics features in the SmartPOS system.
    /// Provides endpoints for business insights, sales analysis, and demand forecasting.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AIController : ControllerBase
    {
        private readonly IAIChatbotService _aiChatbotService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AIController"/> class.
        /// </summary>
        /// <param name="aiChatbotService">The AI chatbot service instance injected via DI.</param>
        public AIController(IAIChatbotService aiChatbotService)
        {
            _aiChatbotService = aiChatbotService;
        }

        /// <summary>
        /// Retrieves an AI-generated business insight based on a natural-language query.
        /// </summary>
        /// <param name="query">The natural-language query string.</param>
        /// <returns>An ApiResponse containing the AI-generated insight text.</returns>
        /// <response code="200">Returns the generated insight.</response>
        [HttpPost("insight")]
        public async Task<IActionResult> GetInsight([FromBody] string query)
        {
            var result = await _aiChatbotService.GetInsight(query);
            return Ok(result);
        }

        /// <summary>
        /// Performs AI-driven analysis on sales data matching the specified filter criteria.
        /// </summary>
        /// <param name="filter">The filter criteria including date range, period, user, sale type, and status.</param>
        /// <returns>An ApiResponse containing the AI-generated sales analytics.</returns>
        /// <response code="200">Returns the sales analysis results.</response>
        [HttpPost("analyze-sales")]
        public async Task<IActionResult> AnalyzeSales([FromBody] SaleFilterDto filter)
        {
            var result = await _aiChatbotService.AnalyzeSales(filter);
            return Ok(result);
        }

        /// <summary>
        /// Forecasts future demand for a specific product using AI prediction models.
        /// </summary>
        /// <param name="productId">The product ID to forecast demand for.</param>
        /// <returns>An ApiResponse containing the AI-generated demand forecast.</returns>
        /// <response code="200">Returns the demand forecast.</response>
        [HttpGet("forecast/{productId}")]
        public async Task<IActionResult> ForecastDemand(int productId)
        {
            var result = await _aiChatbotService.ForecastDemand(productId);
            return Ok(result);
        }
    }
}
