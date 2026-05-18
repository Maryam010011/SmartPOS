using System;
using System.Collections.Generic;

namespace SmartPOS.Models;

public partial class Customer
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int LoyaltyPoints { get; set; }

    public decimal TotalSpent { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual User User { get; set; } = null!;
}
