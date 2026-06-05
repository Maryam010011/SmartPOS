using System;

namespace SmartPOS.Models;

public partial class AuditLog
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Action { get; set; } = null!;
    public string Module { get; set; } = null!;
    public int EntityId { get; set; }
    public string? OldValues { get; set; }
    public string? NewValues { get; set; }
    public DateTime Timestamp { get; set; }
    public string? IPAddress { get; set; }
    public string? Details { get; set; }

    public virtual User User { get; set; } = null!;
}
