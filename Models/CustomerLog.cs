using System;
using System.Collections.Generic;

namespace SmartPOS.Models;

public partial class CustomerLog
{
    public int Id { get; set; }

    public int CustomerId { get; set; }

    public string ActionDetails { get; set; } = null!;

    public DateTime Timestamp { get; set; }

    public virtual User Customer { get; set; } = null!;
}
