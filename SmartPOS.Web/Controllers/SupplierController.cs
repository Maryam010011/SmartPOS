using Microsoft.AspNetCore.Mvc;
using SmartPOS.Shared.DTOs.Suppliers;
using SmartPOS.Shared.Interfaces;

namespace SmartPOS.Web.Controllers
{
    /// <summary>
    /// API controller for managing suppliers in the SmartPOS system.
    /// Provides endpoints for CRUD operations on supplier records.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class SupplierController : ControllerBase
    {
        private readonly ISupplierService _supplierService;

        /// <summary>
        /// Initializes a new instance of the <see cref="SupplierController"/> class.
        /// </summary>
        /// <param name="supplierService">The supplier service instance injected via DI.</param>
        public SupplierController(ISupplierService supplierService)
        {
            _supplierService = supplierService;
        }

        /// <summary>
        /// Retrieves all suppliers.
        /// </summary>
        /// <returns>An ApiResponse containing a list of all suppliers.</returns>
        /// <response code="200">Returns the list of suppliers.</response>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _supplierService.GetAll();
            return Ok(result);
        }

        /// <summary>
        /// Retrieves a specific supplier by its unique identifier.
        /// </summary>
        /// <param name="id">The supplier ID.</param>
        /// <returns>An ApiResponse containing the requested supplier.</returns>
        /// <response code="200">Returns the supplier.</response>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _supplierService.GetById(id);
            return Ok(result);
        }

        /// <summary>
        /// Creates a new supplier.
        /// </summary>
        /// <param name="dto">The supplier creation data.</param>
        /// <returns>An ApiResponse containing the newly created supplier.</returns>
        /// <response code="200">Returns the created supplier.</response>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateSupplierDto dto)
        {
            var result = await _supplierService.Create(dto);
            return Ok(result);
        }

        /// <summary>
        /// Updates an existing supplier.
        /// </summary>
        /// <param name="dto">The supplier update data including the supplier ID.</param>
        /// <returns>An ApiResponse containing the updated supplier.</returns>
        /// <response code="200">Returns the updated supplier.</response>
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] SupplierDto dto)
        {
            var result = await _supplierService.Update(dto);
            return Ok(result);
        }

        /// <summary>
        /// Deletes a supplier by its unique identifier.
        /// </summary>
        /// <param name="id">The supplier ID to delete.</param>
        /// <returns>An ApiResponse indicating success or failure.</returns>
        /// <response code="200">Returns the deletion result.</response>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _supplierService.Delete(id);
            return Ok(result);
        }
    }
}
