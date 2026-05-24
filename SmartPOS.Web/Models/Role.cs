using System.Collections.Generic;

namespace SmartPOS.Web.Models;

public class Role
{
    public int Id { get; set; }
    public string RoleName { get; set; } = string.Empty;
    // Permissions stored as JSON string (nvarchar(max))
    public string Permissions { get; set; } = "{}";
    // Navigation property to Users for UserCount queries
    public ICollection<User>? Users { get; set; }
}
