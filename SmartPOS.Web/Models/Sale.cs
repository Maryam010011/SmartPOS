using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SmartPOS.Shared.Enums;

namespace SmartPOS.Web.Models
{
    [Table("Sale")]
    public class Sale
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        // Nullable — walk-in customers may not have an account
        public int? CustomerId { get; set; }

        [Required]
        public int UserId { get; set; }

        // Nullable — sale may have no promo applied
        public int? PromoId { get; set; }

        [Required]
        public SaleType SaleType { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal DiscountAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TaxAmount { get; set; }

        public DateTime SaleDate { get; set; } = DateTime.UtcNow;

        [Required]
        public SaleStatus Status { get; set; }

        // ── Navigation Properties ──
        // TODO: Uncomment when Customer, User, Promotion models are added
        // [ForeignKey(nameof(CustomerId))]
        // public virtual Customer? Customer { get; set; }

        // [ForeignKey(nameof(UserId))]
        // public virtual User User { get; set; } = null!;

        // [ForeignKey(nameof(PromoId))]
        // public virtual Promotion? Promo { get; set; }

        public virtual ICollection<SaleItem> SaleItems { get; set; } = new List<SaleItem>();
    }
}
