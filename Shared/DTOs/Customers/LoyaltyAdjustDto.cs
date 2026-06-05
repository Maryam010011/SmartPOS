namespace SmartPOS.Shared.DTOs.Customers;

public class LoyaltyAdjustDto
{
    public int Points { get; set; }
    public string Reason { get; set; } = string.Empty;
}
