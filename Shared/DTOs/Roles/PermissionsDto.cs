namespace SmartPOS.Shared.DTOs.Roles;

public class PermissionsDto
{
    public Dictionary<string, ModulePermission> Modules { get; set; } = new();
}
