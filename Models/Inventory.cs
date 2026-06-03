using System;

namespace SmartPOS.Web.Models;

public partial class Inventory
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public int ReorderLevel { get; set; }
    public DateTime LastUpdated { get; set; }

    public virtual Product Product { get; set; } = null!;
}
