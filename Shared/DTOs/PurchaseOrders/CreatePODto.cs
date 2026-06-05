namespace SmartPOS.Shared.DTOs.PurchaseOrders;

public class CreatePODto
{
    public int SupplierId { get; set; }
    public int UserId { get; set; }
    public string? Notes { get; set; }
    public List<CreatePOLineItemDto> Items { get; set; } = new();
    public List<CreatePOLineItemDto> LineItems { get => Items; set => Items = value; }
}

public class CreatePOLineItemDto
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public int OrderedQty { get => Quantity; set => Quantity = value; }
    public decimal UnitPrice { get; set; }
}
