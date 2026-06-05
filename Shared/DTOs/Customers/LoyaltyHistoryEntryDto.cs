namespace SmartPOS.Shared.DTOs.Customers;

public class LoyaltyHistoryEntryDto
{
    public int Points { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
