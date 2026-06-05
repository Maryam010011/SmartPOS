using SmartPOS.Shared.Enums;

namespace SmartPOS.Shared.DTOs.Sales;

public class SaleResultDto
{
    public int Id { get; set; }
    public int SaleId { get; set; }
    public int? CustomerId { get; set; }
    public int UserId { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal SubTotal { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public DateTime SaleDate { get; set; }
    public SaleStatus Status { get; set; }
    public string ReceiptNumber { get; set; } = string.Empty;
    public List<SaleItemDto> Items { get; set; } = new();
}
