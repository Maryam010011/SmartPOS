using System;
using System.Collections.Generic;

namespace SmartPOS.Models;

public partial class Product
{
     public int Id { get; set; }
     public string Name { get; set; } = null!;
     public string SKU { get; set; } = null!;
     public string? Description { get; set; }
     public string? ImageURL { get; set; }
     public decimal Price { get; set; }
     public decimal CostPrice { get; set; }
     public bool IsActive { get; set; }
     public int CategoryId { get; set; }
     public int? SupplierId { get; set; }
     public DateTime CreatedAt { get; set; }

     public virtual Category Category { get; set; } = null!;
     public virtual Supplier? Supplier { get; set; }
     public virtual Inventory? Inventory { get; set; }
     public virtual ICollection<SaleItem> SaleItems { get; set; } = new List<SaleItem>();
     public virtual ICollection<POLineItem> POLineItems { get; set; } = new List<POLineItem>();
     public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
}
