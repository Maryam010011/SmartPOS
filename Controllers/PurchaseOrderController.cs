using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartPOS.Shared.Common;
using SmartPOS.Shared.DTOs.PurchaseOrders;
using SmartPOS.Shared.Interfaces;

namespace SmartPOS.Controllers;

[ApiController]
[Route("api/purchaseorders")]
public class PurchaseOrderController : ControllerBase
{
    private readonly IPurchaseOrderService _purchaseOrderService;

    public PurchaseOrderController(IPurchaseOrderService purchaseOrderService)
    {
        _purchaseOrderService = purchaseOrderService;
    }

    // GET /api/purchaseorders
    [HttpGet]
    [Authorize(Roles = "Manager,Admin")]
    public async Task<IActionResult> GetAll()
    {
        var result = await _purchaseOrderService.GetAll();
        return result.Success ? Ok(result) : BadRequest(result);
    }

    // GET /api/purchaseorders/{id}
    [HttpGet("{id}")]
    [Authorize(Roles = "Manager,Admin")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _purchaseOrderService.GetById(id);
        return result.Success ? Ok(result) : NotFound(result);
    }

    // POST /api/purchaseorders
    [HttpPost]
    [Authorize(Roles = "Manager,Admin")]
    public async Task<IActionResult> Create([FromBody] CreatePODto dto)
    {
        var result = await _purchaseOrderService.Create(dto);
        return result.Success
            ? CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result)
            : BadRequest(result);
    }

    // PUT /api/purchaseorders/{id}/receive
    [HttpPut("{id}/receive")]
    [Authorize(Roles = "Manager,Admin")]
    public async Task<IActionResult> MarkAsReceived(int id, [FromBody] ReceiveRequest request)
    {
        var result = await _purchaseOrderService.MarkAsReceived(id, request.UserId);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    // PUT /api/purchaseorders/{id}/cancel
    [HttpPut("{id}/cancel")]
    [Authorize(Roles = "Manager,Admin")]
    public async Task<IActionResult> Cancel(int id)
    {
        var result = await _purchaseOrderService.Cancel(id);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}
