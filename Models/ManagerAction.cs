using System;
using System.Collections.Generic;

namespace SmartPOS.Models;

public partial class ManagerAction
{
    public int Id { get; set; }

    public int ManagerId { get; set; }

    public string ActionDetails { get; set; } = null!;

    public DateTime Timestamp { get; set; }

    public virtual User Manager { get; set; } = null!;
}
