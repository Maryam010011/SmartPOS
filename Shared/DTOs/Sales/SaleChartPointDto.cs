namespace SmartPOS.Shared.DTOs.Sales;

public class SaleChartPointDto
{
    public string Date { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public decimal Revenue { get; set; }
    public int Orders { get; set; }
    public int Transactions { get; set; }
}
