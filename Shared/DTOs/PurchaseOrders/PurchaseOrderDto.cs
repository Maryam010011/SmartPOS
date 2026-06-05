using SmartPOS.Shared.Enums;

namespace SmartPOS.Shared.DTOs.PurchaseOrders;

public class PurchaseOrderDto
{
    public int Id { get; set; }
    public int SupplierId { get; set; }
    public string SupplierName { get; set; } = string.Empty;
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public POStatus Status { get; set; }
    public decimal TotalCost { get; set; }
    public DateTime OrderDate { get; set; }
    public DateTime? ReceivedAt { get; set; }
    public string? Notes { get; set; }
    public List<POLineItemDto> LineItems { get; set; } = new();
}
