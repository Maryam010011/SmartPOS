using SmartPOS.Shared.Common;
using SmartPOS.Shared.DTOs.Categories;
using SmartPOS.Shared.Interfaces;

namespace SmartPOS.Services.MaryamJ
{
    public class CategoryService : ICategoryService
    {
        public Task<ApiResponse<CategoryDto>> Create(CreateCategoryDto dto)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse> Delete(int id)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<List<CategoryDto>>> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<CategoryDto>> GetById(int id)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<List<CategoryDto>>> GetTree()
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<CategoryDto>> Update(UpdateCategoryDto dto)
        {
            throw new NotImplementedException();
        }
    }
}
