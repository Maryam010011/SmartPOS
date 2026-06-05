using System.Collections.Generic;

namespace SmartPOS.Models;

public partial class Role
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string RoleName { get => Name; set => Name = value; }
    public string Permissions { get; set; } = "{}";

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
