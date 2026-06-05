using SmartPOS.Shared.Common;
using SmartPOS.Shared.DTOs.PurchaseOrders;

namespace SmartPOS.Shared.Interfaces;

public interface IPurchaseOrderService
{
    Task<ApiResponse<List<PurchaseOrderDto>>> GetAll();
    Task<ApiResponse<PurchaseOrderDto>> GetById(int id);
    Task<ApiResponse<PurchaseOrderDto>> Create(CreatePODto dto);
    Task<ApiResponse> MarkAsReceived(int id, int userId);
    Task<ApiResponse> Cancel(int id);
}
