using System;
namespace SmartPOS.Web.Models;
public partial class Review
{
     public int Id { get; set; }
     public int CustomerId { get; set; }
     public int ProductId { get; set; }
     public int Rating { get; set; }
     public string? Comment { get; set; }
     public string? Sentiment { get; set; }
     public double? SentimentScore { get; set; }
     public DateTime CreatedAt { get; set; }
     public virtual Customer Customer { get; set; } = null!;
     public virtual Product Product { get; set; } = null!;
}