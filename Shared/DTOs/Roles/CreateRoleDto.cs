namespace SmartPOS.Shared.DTOs.Roles;

public class CreateRoleDto
{
    public string Name { get; set; } = string.Empty;
    public string RoleName { get; set; } = string.Empty;
    public string Permissions { get; set; } = "{}";
}
