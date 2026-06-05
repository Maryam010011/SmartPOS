namespace SmartPOS.Shared.DTOs.Users;

public class UserDetailDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int RoleId { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<string> LoginHistory { get; set; } = new();
    public string AuditSummary { get; set; } = string.Empty;
}
