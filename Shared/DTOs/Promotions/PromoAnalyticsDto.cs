namespace SmartPOS.Shared.DTOs.Promotions;

public class PromoAnalyticsDto
{
    public int PromotionId { get; set; }
    public string Code { get; set; } = string.Empty;
    public int TotalUsage { get; set; }
    public int TotalTimesUsed { get; set; }
    public decimal TotalDiscountGiven { get; set; }
    public decimal RevenueFromPromoOrders { get; set; }
    public List<DailyUsageStat> DailyStats { get; set; } = new();
    public List<DailyUsageStat> DailyUsageList { get; set; } = new();
}

public class DailyUsageStat
{
    public DateOnly Date { get; set; }
    public int UsageCount { get; set; }
    public decimal DiscountGiven { get; set; }
    public decimal TotalDiscount { get => DiscountGiven; set => DiscountGiven = value; }
    public decimal TotalRevenue { get; set; }
}
