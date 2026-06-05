using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartPOS.Shared.Common;
using SmartPOS.Shared.DTOs.Inventory;
using SmartPOS.Shared.Interfaces;

namespace SmartPOS.Controllers;

[ApiController]
[Route("api/inventory")]
public class InventoryController : ControllerBase
{
    private readonly IInventoryService _inventoryService;

    public InventoryController(IInventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }

    // GET /api/inventory
    [HttpGet]
    [Authorize(Roles = "Manager,Admin")]
    public async Task<IActionResult> GetAll()
    {
        var result = await _inventoryService.GetAll();
        return result.Success ? Ok(result) : BadRequest(result);
    }

    // GET /api/inventory/low-stock
    [HttpGet("low-stock")]
    [Authorize(Roles = "Manager,Admin")]
    public async Task<IActionResult> GetLowStock()
    {
        var result = await _inventoryService.GetLowStock();
        return result.Success ? Ok(result) : BadRequest(result);
    }

    // GET /api/inventory/{productId}
    [HttpGet("{productId}")]
    [Authorize(Roles = "Manager,Admin")]
    public async Task<IActionResult> GetByProduct(int productId)
    {
        var result = await _inventoryService.GetByProduct(productId);
        return result.Success ? Ok(result) : NotFound(result);
    }

    // PUT /api/inventory/adjust
    [HttpPut("adjust")]
    [Authorize(Roles = "Manager,Admin")]
    public async Task<IActionResult> AdjustStock([FromBody] AdjustStockDto dto)
    {
        var result = await _inventoryService.AdjustStock(dto);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    // PUT /api/inventory/add
    [HttpPut("add")]
    [Authorize(Roles = "Manager,Admin")]
    public async Task<IActionResult> AddStock([FromBody] AddStockRequest request)
    {
        var result = await _inventoryService.AddStock(request.ProductId, request.Quantity);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    // PUT /api/inventory/deduct
    [HttpPut("deduct")]
    [Authorize(Roles = "Manager,Admin")]
    public async Task<IActionResult> DeductStock([FromBody] DeductStockRequest request)
    {
        var result = await _inventoryService.DeductStock(request.ProductId, request.Quantity);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}
