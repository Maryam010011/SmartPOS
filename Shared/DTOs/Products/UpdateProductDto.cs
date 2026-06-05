namespace SmartPOS.Shared.DTOs.Products;

public class UpdateProductDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string SKU { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public decimal CostPrice { get; set; }
    public string? ImageURL { get; set; }
    public int CategoryId { get; set; }
    public int? SupplierId { get; set; }
    public bool IsActive { get; set; }
}
