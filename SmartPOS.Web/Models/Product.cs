using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartPOS.Web.Models
{
    [Table("Product")]
    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string SKU { get; set; } = string.Empty;

        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal CostPrice { get; set; }

        [StringLength(500)]
        public string ImageURL { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        // ── Foreign Keys ──

        [Required]
        public int CategoryId { get; set; }

        public int? SupplierId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // ── Navigation Properties ──

        [ForeignKey(nameof(CategoryId))]
        public virtual Category Category { get; set; } = null!;

        [ForeignKey(nameof(SupplierId))]
        public virtual Supplier? Supplier { get; set; }

        public virtual ICollection<SaleItem> SaleItems { get; set; } = new List<SaleItem>();

        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}
