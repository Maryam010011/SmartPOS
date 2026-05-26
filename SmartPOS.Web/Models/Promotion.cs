using System;
using System.Collections.Generic;
using SmartPOS.Shared.Enums;

namespace SmartPOS.Web.Models;

public partial class Promotion
{
    public int Id { get; set; }
    public string Code { get; set; } = null!;
    public DiscountType DiscountType { get; set; }
    public decimal Value { get; set; }
    public decimal MinOrderValue { get; set; }
    public int? MaxUsageLimit { get; set; }
    public int UsageCount { get; set; }
    public DateOnly ValidFrom { get; set; }
    public DateOnly ValidTo { get; set; }
    public bool IsActive { get; set; }

    public virtual ICollection<Sale> Sales { get; set; } = new List<Sale>();
}
