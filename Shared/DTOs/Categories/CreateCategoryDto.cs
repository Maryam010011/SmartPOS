namespace SmartPOS.Shared.DTOs.Categories;

public class CreateCategoryDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ImageURL { get; set; }
    public int? ParentCategoryId { get; set; }
}
