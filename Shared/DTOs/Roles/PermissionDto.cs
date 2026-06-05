namespace SmartPOS.Shared.DTOs.Roles;

public class PermissionDto
{
    public int Id { get; set; }
    public string Module { get; set; } = string.Empty;
    public bool CanCreate { get; set; }
    public bool CanRead { get; set; }
    public bool CanUpdate { get; set; }
    public bool CanDelete { get; set; }
}
