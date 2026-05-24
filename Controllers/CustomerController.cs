using Microsoft.AspNetCore.Mvc;
using SmartPOS.Shared.Common;
using SmartPOS.Shared.DTOs.Customers;
using SmartPOS.Shared.Interfaces;

namespace SmartPOS.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomerController : ControllerBase
{
    private readonly ICustomerService _customerService;

    public CustomerController(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    // GET /api/customers
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] CustomerFilterDto filter)
    {
        var result = await _customerService.GetAllCustomers(filter);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    // GET /api/customers/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _customerService.GetCustomerById(id);
        return result.Success ? Ok(result) : NotFound(result);
    }

    // POST /api/customers
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCustomerDto dto)
    {
        var result = await _customerService.Create(dto);
        return result.Success
            ? CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result)
            : BadRequest(result);
    }

    // PUT /api/customers/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateCustomerDto dto)
    {
        var result = await _customerService.Update(id, dto);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    // DELETE /api/customers/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _customerService.DeleteCustomer(id);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    // POST /api/customers/{id}/loyalty/add
    [HttpPost("{id}/loyalty/add")]
    public async Task<IActionResult> AddLoyaltyPoints(int id, [FromBody] LoyaltyAdjustDto dto)
    {
        var result = await _customerService.AddLoyaltyPoints(id, dto.Points);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    // POST /api/customers/{id}/loyalty/deduct
    [HttpPost("{id}/loyalty/deduct")]
    public async Task<IActionResult> DeductLoyaltyPoints(int id, [FromBody] LoyaltyAdjustDto dto)
    {
        var result = await _customerService.DeductLoyaltyPoints(id, dto.Points);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    // PUT /api/customers/{id}/loyalty/adjust
    [HttpPut("{id}/loyalty/adjust")]
    public async Task<IActionResult> AdjustLoyaltyPoints(int id, [FromBody] LoyaltyAdjustDto dto)
    {
        var result = await _customerService.AdjustLoyaltyPoints(id, dto.Points, dto.Reason);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    // GET /api/customers/{id}/orders
    [HttpGet("{id}/orders")]
    public async Task<IActionResult> GetPurchaseHistory(int id)
    {
        var result = await _customerService.GetCustomerPurchaseHistory(id);
        return result.Success ? Ok(result) : NotFound(result);
    }
}
