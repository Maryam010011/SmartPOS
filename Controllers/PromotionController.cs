using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartPOS.Shared.Common;
using SmartPOS.Shared.DTOs.Promotions;
using SmartPOS.Shared.Interfaces;

namespace SmartPOS.Controllers;

[ApiController]
[Route("api/promotions")]
public class PromotionController : ControllerBase
{
    private readonly IPromotionService _promotionService;

    public PromotionController(IPromotionService promotionService)
    {
        _promotionService = promotionService;
    }

    // GET /api/promotions
    [HttpGet]
    [Authorize(Roles = "Manager,Admin")]
    public async Task<IActionResult> GetAll()
    {
        var result = await _promotionService.GetAllPromotions();
        return result.Success ? Ok(result) : BadRequest(result);
    }

    // GET /api/promotions/{id}
    [HttpGet("{id}")]
    [Authorize(Roles = "Manager,Admin")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _promotionService.GetById(id);
        return result.Success ? Ok(result) : NotFound(result);
    }

    // POST /api/promotions
    [HttpPost]
    [Authorize(Roles = "Manager,Admin")]
    public async Task<IActionResult> Create([FromBody] CreatePromotionDto dto)
    {
        var result = await _promotionService.CreatePromotion(dto);
        return result.Success
            ? CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result)
            : BadRequest(result);
    }

    // PUT /api/promotions/{id}
    [HttpPut("{id}")]
    [Authorize(Roles = "Manager,Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdatePromotionDto dto)
    {
        var result = await _promotionService.UpdatePromotion(id, dto);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    // DELETE /api/promotions/{id}
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _promotionService.DeletePromotion(id);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    // PUT /api/promotions/{id}/toggle
    [HttpPut("{id}/toggle")]
    [Authorize(Roles = "Manager,Admin")]
    public async Task<IActionResult> ToggleActive(int id)
    {
        var result = await _promotionService.ToggleActive(id);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    // POST /api/promotions/validate
    [HttpPost("validate")]
    [Authorize(Roles = "Cashier,Manager,Admin")]
    public async Task<IActionResult> Validate([FromBody] ValidatePromoRequest request)
    {
        var result = await _promotionService.ValidatePromoCode(request.Code, request.OrderTotal);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    // GET /api/promotions/{id}/analytics
    [HttpGet("{id}/analytics")]
    [Authorize(Roles = "Manager,Admin")]
    public async Task<IActionResult> GetAnalytics(int id)
    {
        var result = await _promotionService.GetPromoAnalytics(id);
        return result.Success ? Ok(result) : NotFound(result);
    }
}

public class ValidatePromoRequest
{
    public string Code { get; set; } = string.Empty;
    public decimal OrderTotal { get; set; }
}
