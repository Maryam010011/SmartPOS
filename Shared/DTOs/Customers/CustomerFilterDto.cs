namespace SmartPOS.Shared.DTOs.Customers;

public class CustomerFilterDto
{
    public bool? IsActive { get; set; }
    public string? SearchTerm { get; set; }
    public DateTime? RegistrationDateFrom { get; set; }
    public DateTime? RegistrationDateTo { get; set; }
    public decimal? MinSpend { get; set; }
    public decimal? MaxSpend { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
