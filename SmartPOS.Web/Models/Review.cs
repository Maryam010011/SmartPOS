using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartPOS.Web.Models
{
    [Table("Review")]
    public class Review
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int CustomerId { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public int Rating { get; set; }

        [StringLength(2000)]
        public string Comment { get; set; } = string.Empty;

        [StringLength(20)]
        public string Sentiment { get; set; } = string.Empty; // Positive | Negative | Neutral

        public float SentimentScore { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // ── Navigation Properties ──

        [ForeignKey(nameof(CustomerId))]
        public virtual object Customer { get; set; } = null!;

        [ForeignKey(nameof(ProductId))]
        public virtual Product Product { get; set; } = null!;
    }
}
