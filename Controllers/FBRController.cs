using Microsoft.AspNetCore.Mvc;
using SmartPOS.Shared.Interfaces;
using SmartPOS.Shared.DTOs;
using SmartPOS.Shared.DTOs.Sales;

namespace SmartPOS.Web.Controllers
{
    /// <summary>
    /// API controller for FBR (Federal Board of Revenue) tax integration.
    /// Provides endpoints for calculating GST tax, submitting sale invoices to FBR,
    /// and checking the status of submitted invoices.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class FBRController : ControllerBase
    {
        private readonly IFBRService _fbrService;

        /// <summary>
        /// Initializes a new instance of the <see cref="FBRController"/> class.
        /// </summary>
        /// <param name="fbrService">The FBR service instance injected via DI.</param>
        public FBRController(IFBRService fbrService)
        {
            _fbrService = fbrService;
        }

        /// <summary>
        /// Calculates the GST tax for a given amount.
        /// </summary>
        /// <param name="amount">The sale amount on which to calculate tax.</param>
        /// <returns>An ApiResponse containing the calculated tax amount.</returns>
        /// <response code="200">Returns the calculated tax value.</response>
        [HttpPost("calculate-tax")]
        public async Task<IActionResult> CalculateTax([FromBody] decimal amount)
        {
            var result = await _fbrService.CalculateTax(amount);
            return Ok(result);
        }

        /// <summary>
        /// Submits a sale invoice to the FBR for tax reporting.
        /// </summary>
        /// <param name="sale">The sale result data to submit as an FBR invoice.</param>
        /// <returns>An ApiResponse containing the FBR invoice reference string.</returns>
        /// <response code="200">Returns the FBR submission reference.</response>
        [HttpPost("submit-invoice")]
        public async Task<IActionResult> SubmitInvoice([FromBody] SaleResultDto sale)
        {
            var result = await _fbrService.SubmitInvoice(sale);
            return Ok(result);
        }

        /// <summary>
        /// Retrieves the tax status of a previously submitted FBR invoice.
        /// </summary>
        /// <param name="reference">The FBR invoice reference string returned from submission.</param>
        /// <returns>An ApiResponse containing the current tax status of the invoice.</returns>
        /// <response code="200">Returns the invoice tax status.</response>
        [HttpGet("tax-status")]
        public async Task<IActionResult> GetTaxStatus([FromQuery(Name = "ref")] string reference)
        {
            var result = await _fbrService.GetTaxStatus(reference);
            return Ok(result);
        }
    }
}
