using SmartPOS.Shared.Common;
using SmartPOS.Shared.DTOs.Sales;

namespace SmartPOS.Shared.Interfaces;

public interface ISaleService
{
    Task<ApiResponse<SaleResultDto>> ProcessSale(CreateSaleDto dto);
    Task<ApiResponse<List<SaleResultDto>>> GetAll(SaleFilterDto filter);
    Task<ApiResponse<SaleResultDto>> GetById(int id);
    Task<ApiResponse<SaleAnalyticsDto>> GetAnalytics(SaleFilterDto filter);
    Task<ApiResponse> VoidSale(int saleId, string reason);
    Task<ApiResponse> RefundSale(int saleId);
}
