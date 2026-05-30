using SmartPOS.Shared.Common;
using SmartPOS.Shared.DTOs.PurchaseOrders;
using SmartPOS.Shared.Interfaces;

namespace SmartPOS.Services.MaryamJ
{
    public class PurchaseOrderService : IPurchaseOrderService
    {
        public Task<ApiResponse> Cancel(int poId)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<PurchaseOrderDto>> Create(CreatePODto dto)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<List<PurchaseOrderDto>>> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<PurchaseOrderDto>> GetById(int id)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse> MarkAsReceived(int poId, int userId)
        {
            throw new NotImplementedException();
        }
    }
}
