namespace SmartPOS.Shared.DTOs.Reviews;

public class ReviewDto
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public string? Sentiment { get; set; }
    public double SentimentScore { get; set; }
    public DateTime CreatedAt { get; set; }
}
