using SmartPOS.Shared.Common;
using SmartPOS.Shared.DTOs.Inventory;
using SmartPOS.Shared.Interfaces;

namespace SmartPOS.Services.MaryamJ
{
    public class InventoryService : IInventoryService
    {
        public Task<ApiResponse> AddStock(int productId, int quantity)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse> AdjustStock(AdjustStockDto dto)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse> DeductStock(int productId, int quantity)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<List<InventoryDto>>> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<InventoryDto>> GetByProduct(int productId)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<List<InventoryDto>>> GetLowStock()
        {
            throw new NotImplementedException();
        }
    }
}
