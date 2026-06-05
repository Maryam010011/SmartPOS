using SmartPOS.Shared.DTOs.Roles;

namespace SmartPOS.Shared.DTOs.Roles;

public class UpdateRoleDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string RoleName { get; set; } = string.Empty;
    public PermissionsDto? Permissions { get; set; }
}
