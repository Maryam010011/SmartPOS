namespace SmartPOS.Shared.DTOs.Customers;

public class CustomerDetailDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    public string? Address { get; set; }
    public bool IsActive { get; set; }
    public int LoyaltyPoints { get; set; }
    public decimal TotalSpent { get; set; }
    public int TotalOrders { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<SaleSummaryDto> RecentOrders { get; set; } = new();
    public List<LoyaltyHistoryEntryDto> LoyaltyHistory { get; set; } = new();
}
