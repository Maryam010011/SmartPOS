using SmartPOS.Shared.Enums;

namespace SmartPOS.Shared.DTOs.Sales;

public class CreateSaleDto
{
    public int? CustomerId { get; set; }
    public int UserId { get; set; }
    public string? PromoCode { get; set; }
    public SaleType SaleType { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public List<SaleItemDto> Items { get; set; } = new();
}

public class SaleItemDto
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal LineTotal { get; set; }
}
