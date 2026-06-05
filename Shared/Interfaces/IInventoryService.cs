using SmartPOS.Shared.Common;
using SmartPOS.Shared.DTOs.Inventory;

namespace SmartPOS.Shared.Interfaces;

public interface IInventoryService
{
    Task<ApiResponse<List<InventoryDto>>> GetAll();
    Task<ApiResponse<InventoryDto>> GetByProduct(int productId);
    Task<ApiResponse<List<InventoryDto>>> GetLowStock();
    Task<ApiResponse> AdjustStock(AdjustStockDto dto);
    Task<ApiResponse> AddStock(int productId, int quantity);
    Task<ApiResponse> DeductStock(int productId, int quantity);
}
