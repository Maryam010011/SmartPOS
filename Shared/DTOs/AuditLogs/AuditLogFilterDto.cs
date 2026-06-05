namespace SmartPOS.Shared.DTOs.AuditLogs;

public class AuditLogFilterDto
{
    public int? UserId { get; set; }
    public string? Module { get; set; }
    public string? Action { get; set; }
    public string? SearchTerm { get; set; }
    public DateTime? From { get; set; }
    public DateTime? DateFrom { get => From; set => From = value; }
    public DateTime? To { get; set; }
    public DateTime? DateTo { get => To; set => To = value; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
