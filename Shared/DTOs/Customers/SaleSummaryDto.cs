namespace SmartPOS.Shared.DTOs.Customers;

public class SaleSummaryDto
{
    public int Id { get; set; }
    public int SaleId => Id;
    public DateTime SaleDate { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public int ItemCount { get; set; }
}
