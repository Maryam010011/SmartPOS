using System.Collections.Generic;

namespace SmartPOS.Models;

public partial class Role
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string RoleName { get => Name; set => Name = value; }
    
    // Rename this property to something else if you are using it to store JSON strings
    // so it doesn't conflict with EF Core's expectation of a collection of Permission entities.
    public string PermissionsJson { get; set; } = "{}";

    public virtual ICollection<User> Users { get; set; } = new List<User>();
    
    // Optional: If you want the one-to-many relationship navigation to the Permission entity table
    public virtual ICollection<Permission> Permissions { get; set; } = new List<Permission>();
    
    // Alias kept for compatibility
    public virtual ICollection<Permission> RolePermissions => Permissions;
}
