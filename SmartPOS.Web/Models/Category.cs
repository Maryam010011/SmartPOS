using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartPOS.Web.Models
{
    [Table("Category")]
    public class Category
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        // Self-referencing FK — nullable for root categories
        public int? ParentCategoryId { get; set; }

        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        [StringLength(500)]
        public string ImageURL { get; set; } = string.Empty;

        // ── Navigation Properties ──

        [ForeignKey(nameof(ParentCategoryId))]
        public virtual Category? ParentCategory { get; set; }

        public virtual ICollection<Category> SubCategories { get; set; } = new List<Category>();

        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
