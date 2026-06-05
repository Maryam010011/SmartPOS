using System;

namespace SmartPOS.Models;

public partial class LoyaltyTransaction
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public int Points { get; set; }
    public string Type { get; set; } = null!;
    public string Reason { get; set; } = null!;
    public DateTime CreatedAt { get; set; }

    public virtual Customer Customer { get; set; } = null!;
}
