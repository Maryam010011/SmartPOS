namespace SmartPOS.Models;

public partial class POLineItem
{
    public int Id { get; set; }
    public int POID { get; set; }
    public int ProductId { get; set; }
    public int OrderedQty { get; set; }
    public decimal UnitPrice { get; set; }

    public virtual PurchaseOrder PurchaseOrder { get; set; } = null!;
    public virtual Product Product { get; set; } = null!;
}
