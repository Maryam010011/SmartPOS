namespace SmartPOS.Models;

public partial class Permission
{
    public int Id { get; set; }
    public int RoleId { get; set; }
    public string Module { get; set; } = null!;
    public bool CanCreate { get; set; }
    public bool CanRead { get; set; }
    public bool CanUpdate { get; set; }
    public bool CanDelete { get; set; }

    public virtual Role Role { get; set; } = null!;
}
