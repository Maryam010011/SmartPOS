using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SmartPOS.Shared.Enums;

namespace SmartPOS.Web.Models
{
    [Table("PurchaseOrder")]
    public class PurchaseOrder
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int SupplierId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public POStatus Status { get; set; } = POStatus.Draft;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalCost { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        public DateTime? ReceivedAt { get; set; }

        public string Notes { get; set; } = string.Empty;

        // ── Navigation Properties ──

        [ForeignKey(nameof(SupplierId))]
        public virtual Supplier? Supplier { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual User? User { get; set; }

        public virtual ICollection<POLineItem> LineItems { get; set; } = new List<POLineItem>();
    }
}
