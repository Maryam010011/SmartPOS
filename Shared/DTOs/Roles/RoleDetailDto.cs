namespace SmartPOS.Shared.DTOs.Roles;

public class RoleDetailDto
{
    public int Id { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public int UserCount { get; set; }
    public PermissionsDto Permissions { get; set; } = new();
}
