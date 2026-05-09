namespace SmartPOS.Models;

public class Customer
{
    public int Id { get; set; }
    
    public int UserId { get; set; }
    public User? User { get; set; }

    public int LoyaltyPoints { get; set; } = 0;
    public decimal TotalSpent { get; set; } = 0;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
