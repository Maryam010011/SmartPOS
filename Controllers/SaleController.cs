using Microsoft.AspNetCore.Mvc;
using SmartPOS.Shared.DTOs.Sales;
using SmartPOS.Shared.Interfaces;

namespace SmartPOS.Web.Controllers
{
    /// <summary>
    /// API controller for managing sales and transactions in the SmartPOS system.
    /// Provides endpoints for processing sales, retrieving sale records, analytics, voiding, and refunding.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class SaleController : ControllerBase
    {
        private readonly ISaleService _saleService;

        /// <summary>
        /// Initializes a new instance of the <see cref="SaleController"/> class.
        /// </summary>
        /// <param name="saleService">The sale service instance injected via DI.</param>
        public SaleController(ISaleService saleService)
        {
            _saleService = saleService;
        }

        /// <summary>
        /// Processes a new sale transaction.
        /// </summary>
        /// <param name="dto">The sale creation data including items, customer, and payment details.</param>
        /// <returns>An ApiResponse containing the processed sale result with receipt number and totals.</returns>
        /// <response code="200">Returns the processed sale result.</response>
        [HttpPost]
        public async Task<IActionResult> ProcessSale([FromBody] CreateSaleDto dto)
        {
            var result = await _saleService.ProcessSale(dto);
            return Ok(result);
        }

        /// <summary>
        /// Retrieves a specific sale by its unique identifier.
        /// </summary>
        /// <param name="id">The sale ID.</param>
        /// <returns>An ApiResponse containing the requested sale record.</returns>
        /// <response code="200">Returns the sale record.</response>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _saleService.GetById(id);
            return Ok(result);
        }

        /// <summary>
        /// Retrieves all sales matching the specified filter criteria.
        /// </summary>
        /// <param name="filter">The filter criteria including date range, period, user, sale type, and status.</param>
        /// <returns>An ApiResponse containing a list of matching sale records.</returns>
        /// <response code="200">Returns the filtered list of sales.</response>
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] SaleFilterDto filter)
        {
            var result = await _saleService.GetAll(filter);
            return Ok(result);
        }

        /// <summary>
        /// Retrieves sales analytics (revenue, transaction count, average order value, chart data) for the given filter.
        /// </summary>
        /// <param name="filter">The filter criteria including date range, period, user, sale type, and status.</param>
        /// <returns>An ApiResponse containing the analytics data.</returns>
        /// <response code="200">Returns the sales analytics.</response>
        [HttpGet("analytics")]
        public async Task<IActionResult> GetAnalytics([FromQuery] SaleFilterDto filter)
        {
            var result = await _saleService.GetAnalytics(filter);
            return Ok(result);
        }

        /// <summary>
        /// Voids a sale transaction with a specified reason.
        /// </summary>
        /// <param name="id">The sale ID to void.</param>
        /// <param name="reason">The reason for voiding the sale.</param>
        /// <returns>An ApiResponse indicating success or failure of the void operation.</returns>
        /// <response code="200">Returns the void result.</response>
        [HttpPatch("{id}/void")]
        public async Task<IActionResult> VoidSale(int id, [FromBody] string reason)
        {
            var result = await _saleService.VoidSale(id, reason);
            return Ok(result);
        }

        /// <summary>
        /// Refunds a sale transaction.
        /// </summary>
        /// <param name="id">The sale ID to refund.</param>
        /// <returns>An ApiResponse indicating success or failure of the refund operation.</returns>
        /// <response code="200">Returns the refund result.</response>
        [HttpPatch("{id}/refund")]
        public async Task<IActionResult> RefundSale(int id)
        {
            var result = await _saleService.RefundSale(id);
            return Ok(result);
        }
    }
}
