using System;
using System.Collections.Generic;

namespace SmartPOS.Models;

public partial class CashierLog
{
    public int Id { get; set; }

    public int CashierId { get; set; }

    public string ActionDetails { get; set; } = null!;

    public DateTime Timestamp { get; set; }

    public virtual User Cashier { get; set; } = null!;
}
