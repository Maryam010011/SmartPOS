using System;
using SmartPOS.Shared.Enums;

namespace SmartPOS.Web.Models;

public partial class Payment
{
    public int Id { get; set; }
    public int SaleId { get; set; }
    public PaymentMethod Method { get; set; }
    public decimal Amount { get; set; }
    public PaymentStatus Status { get; set; }
    public string? TransactionRef { get; set; }
    public DateTime? PaidAt { get; set; }

    public virtual Sale Sale { get; set; } = null!;
}
