namespace SmartPOS.Shared.DTOs.Inventory;

public class InventoryDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public int ReorderLevel { get; set; }
    public bool IsLowStock => Quantity <= ReorderLevel;
    public DateTime LastUpdated { get; set; }
}
