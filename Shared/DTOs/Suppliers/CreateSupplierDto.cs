namespace SmartPOS.Shared.DTOs.Suppliers;

public class CreateSupplierDto
{
    public string Name { get; set; } = string.Empty;
    public string? ContactPerson { get; set; }
    public string? ContactNo { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
}
