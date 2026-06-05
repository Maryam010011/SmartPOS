using System;
using System.Collections.Generic;
using SmartPOS.Shared.Enums;

namespace SmartPOS.Models;

public partial class Sale
{
     public int Id { get; set; }
     public int? CustomerId { get; set; }
     public int UserId { get; set; }
     public int? PromoId { get; set; }
     public SaleType SaleType { get; set; }
     public decimal TotalAmount { get; set; }
     public decimal DiscountAmount { get; set; }
     public decimal TaxAmount { get; set; }
     public DateTime SaleDate { get; set; }
     public SaleStatus Status { get; set; }

     public virtual Customer? Customer { get; set; }
     public virtual User User { get; set; } = null!;
     public virtual Promotion? Promotion { get; set; }
     public virtual ICollection<SaleItem> SaleItems { get; set; } = new List<SaleItem>();
     public virtual Payment? Payment { get; set; }
}
