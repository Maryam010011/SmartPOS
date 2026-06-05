using SmartPOS.Shared.Common;
using SmartPOS.Shared.DTOs.Categories;

namespace SmartPOS.Shared.Interfaces;

public interface ICategoryService
{
    Task<ApiResponse<CategoryDto>> GetById(int id);
    Task<ApiResponse<List<CategoryDto>>> GetAll();
    Task<ApiResponse<List<CategoryDto>>> GetTree();
    Task<ApiResponse<CategoryDto>> Create(CreateCategoryDto dto);
    Task<ApiResponse<CategoryDto>> Update(UpdateCategoryDto dto);
    Task<ApiResponse> Delete(int id);
}
