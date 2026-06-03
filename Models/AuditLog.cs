using System;

namespace SmartPOS.Web.Models;

public partial class AuditLog
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Action { get; set; } = null!;
    public string Module { get; set; } = null!;
    public DateTime Timestamp { get; set; }
    public string? IPAddress { get; set; }
    public string? Details { get; set; }

    public virtual User User { get; set; } = null!;
}
