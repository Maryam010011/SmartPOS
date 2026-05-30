using SmartPOS.Shared.Common;
using SmartPOS.Shared.DTOs.Products;
using SmartPOS.Shared.Interfaces;

namespace SmartPOS.Services.MaryamJ
{
    public class ProductService : IProductService
    {
        public Task<ApiResponse<ProductDto>> Create(CreateProductDto dto)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse> Delete(int id)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<List<ProductDto>>> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<List<ProductDto>>> GetByCategory(int categoryId)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<ProductDto>> GetById(int id)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<List<ProductDto>>> GetBySupplier(int supplierId)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<List<ProductDto>>> Search(string keyword)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse> ToggleActive(int id)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<ProductDto>> Update(UpdateProductDto dto)
        {
            throw new NotImplementedException();
        }
    }
}
