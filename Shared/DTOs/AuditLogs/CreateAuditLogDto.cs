namespace SmartPOS.Shared.DTOs.AuditLogs;

public class CreateAuditLogDto
{
    public int UserId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string Module { get; set; } = string.Empty;
    public int EntityId { get; set; }
    public string? OldValues { get; set; }
    public string? NewValues { get; set; }
    public string? IPAddress { get; set; }
}
