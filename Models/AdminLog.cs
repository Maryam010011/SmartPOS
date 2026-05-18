using System;
using System.Collections.Generic;

namespace SmartPOS.Models;

public partial class AdminLog
{
    public int Id { get; set; }

    public int AdminId { get; set; }

    public string ActionDetails { get; set; } = null!;

    public DateTime Timestamp { get; set; }

    public virtual User Admin { get; set; } = null!;
}
