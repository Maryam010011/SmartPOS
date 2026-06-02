using System;
using System.Collections.Generic;
using SmartPOS.Shared.Enums;

namespace SmartPOS.Models;

public partial class PurchaseOrder
{
    public int Id { get; set; }
    public int SupplierId { get; set; }
    public int UserId { get; set; }
    public POStatus Status { get; set; }
    public decimal TotalCost { get; set; }
    public DateTime OrderDate { get; set; }
    public DateTime? ReceivedAt { get; set; }
    public string? Notes { get; set; }

    public virtual Supplier Supplier { get; set; } = null!;
    public virtual User User { get; set; } = null!;
    public virtual ICollection<POLineItem> LineItems { get; set; } = new List<POLineItem>();
}
