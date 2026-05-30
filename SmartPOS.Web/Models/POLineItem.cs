using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartPOS.Web.Models
{
    [Table("POLineItem")]
    public class POLineItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int POID { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        public int OrderedQty { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }

        // ── Navigation Properties ──

        [ForeignKey(nameof(POID))]
        public virtual PurchaseOrder? PO { get; set; }

        [ForeignKey(nameof(ProductId))]
        public virtual Product? Product { get; set; }
    }
}
