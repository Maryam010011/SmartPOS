namespace SmartPOS.Shared.DTOs.Reviews;

public class SentimentStatDto
{
    public string ProductName { get; set; } = string.Empty;
    public int TotalReviews { get; set; }
    public int PositiveCount { get; set; }
    public int Positive { get; set; }
    public int NegativeCount { get; set; }
    public int Negative { get; set; }
    public int NeutralCount { get; set; }
    public int Neutral { get; set; }
    public double AverageSentiment { get; set; }
    public double AverageRating { get; set; }
}
