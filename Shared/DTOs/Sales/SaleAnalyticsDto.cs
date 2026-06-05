namespace SmartPOS.Shared.DTOs.Sales;

public class SaleAnalyticsDto
{
    public decimal TotalRevenue { get; set; }
    public int TotalOrders { get; set; }
    public int TotalTransactions { get; set; }
    public decimal AverageOrderValue { get; set; }
    public List<SaleChartPointDto> ChartData { get; set; } = new();
}
