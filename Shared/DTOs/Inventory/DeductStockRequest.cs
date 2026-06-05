namespace SmartPOS.Shared.DTOs.Inventory;

public class DeductStockRequest
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}
