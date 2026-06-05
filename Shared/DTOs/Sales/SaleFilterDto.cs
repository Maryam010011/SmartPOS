using SmartPOS.Shared.Enums;

namespace SmartPOS.Shared.DTOs.Sales;

public class SaleFilterDto
{
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
    public int? CustomerId { get; set; }
    public int? UserId { get; set; }
    public SaleStatus? Status { get; set; }
    public string? Period { get; set; }
    public SaleType? SaleType { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
