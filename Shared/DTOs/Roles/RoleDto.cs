namespace SmartPOS.Shared.DTOs.Roles;

public class RoleDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string RoleName { get; set; } = string.Empty;
    public int UserCount { get; set; }
}
