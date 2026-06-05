using System;
using System.Collections.Generic;

namespace SmartPOS.Models;

public partial class Customer
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? Phone { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    public string? Address { get; set; }
    public bool IsActive { get; set; } = true;
    public int LoyaltyPoints { get; set; }
    public decimal TotalSpent { get; set; }
    public DateTime CreatedAt { get; set; }

    public virtual User User { get; set; } = null!;
    public virtual ICollection<Sale> Sales { get; set; } = new List<Sale>();
    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
    public virtual ICollection<LoyaltyTransaction> LoyaltyTransactions { get; set; } = new List<LoyaltyTransaction>();
}
