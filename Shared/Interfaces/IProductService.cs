using SmartPOS.Shared.Common;
using SmartPOS.Shared.DTOs.Products;

namespace SmartPOS.Shared.Interfaces;

public interface IProductService
{
    Task<ApiResponse<ProductDto>> GetById(int id);
    Task<ApiResponse<List<ProductDto>>> GetAll();
    Task<ApiResponse<List<ProductDto>>> GetByCategory(int categoryId);
    Task<ApiResponse<List<ProductDto>>> GetBySupplier(int supplierId);
    Task<ApiResponse<ProductDto>> Create(CreateProductDto dto);
    Task<ApiResponse<ProductDto>> Update(UpdateProductDto dto);
    Task<ApiResponse> Delete(int id);
    Task<ApiResponse> ToggleActive(int id);
    Task<ApiResponse<List<ProductDto>>> Search(string keyword);
}
