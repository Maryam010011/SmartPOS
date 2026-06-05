using SmartPOS.Shared.Common;
using SmartPOS.Shared.DTOs.Sales;

namespace SmartPOS.Shared.Interfaces;

public interface IAIChatbotService
{
    Task<ApiResponse<string>> GetInsight(string query);
    Task<ApiResponse<SaleAnalyticsDto>> AnalyzeSales(SaleFilterDto filter);
    Task<ApiResponse<string>> ForecastDemand(int productId);
}
