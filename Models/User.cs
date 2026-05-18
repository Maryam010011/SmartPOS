using System;
using System.Collections.Generic;

namespace SmartPOS.Models;

public partial class User
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public int RoleId { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<AdminLog> AdminLogs { get; set; } = new List<AdminLog>();

    public virtual ICollection<CashierLog> CashierLogs { get; set; } = new List<CashierLog>();

    public virtual Customer? Customer { get; set; }

    public virtual ICollection<CustomerLog> CustomerLogs { get; set; } = new List<CustomerLog>();

    public virtual ICollection<ManagerAction> ManagerActions { get; set; } = new List<ManagerAction>();

    public virtual Role Role { get; set; } = null!;
}
