// TEMPORARY STUB — Replace with real InventoryService once MaryamY's database layer is merged
using System.Collections.Generic;
using System.Threading.Tasks;
using SmartPOS.Shared.Common;
using SmartPOS.Shared.DTOs.Inventory;
using SmartPOS.Shared.Interfaces;

namespace SmartPOS.Web.Services.Shahzain
{
    public class InventoryServiceStub : IInventoryService
    {
        public Task<ApiResponse> DeductStock(int productId, int quantity)
        {
            return Task.FromResult(ApiResponse.Ok("Stub: Stock deducted successfully"));
        }

        public Task<ApiResponse> AddStock(int productId, int quantity)
        {
            return Task.FromResult(ApiResponse.Ok("Stub: Stock added successfully"));
        }

        public Task<ApiResponse> AdjustStock(AdjustStockDto dto)
        {
            return Task.FromResult(ApiResponse.Ok("Stub: Stock adjusted successfully"));
        }

        public Task<ApiResponse<InventoryDto>> GetByProduct(int productId)
        {
            var stubInventory = new InventoryDto
            {
                ProductId = productId,
                Quantity = 100,
                ReorderLevel = 10,
                LastUpdated = System.DateTime.UtcNow
            };
            return Task.FromResult(ApiResponse<InventoryDto>.Ok(stubInventory, "Stub: Inventory retrieved successfully"));
        }

        public Task<ApiResponse<List<InventoryDto>>> GetAll()
        {
            return Task.FromResult(ApiResponse<List<InventoryDto>>.Ok(new List<InventoryDto>(), "Stub: All inventory retrieved successfully"));
        }

        public Task<ApiResponse<List<InventoryDto>>> GetLowStock()
        {
            return Task.FromResult(ApiResponse<List<InventoryDto>>.Ok(new List<InventoryDto>(), "Stub: Low stock retrieved successfully"));
        }
    }
}
