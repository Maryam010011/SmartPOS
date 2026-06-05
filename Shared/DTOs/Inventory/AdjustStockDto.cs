namespace SmartPOS.Shared.DTOs.Inventory;

public class AdjustStockDto
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public string Reason { get; set; } = string.Empty;
}
