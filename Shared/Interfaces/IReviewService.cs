using SmartPOS.Shared.Common;
using SmartPOS.Shared.DTOs.Reviews;

namespace SmartPOS.Shared.Interfaces;

public interface IReviewService
{
    Task<ApiResponse<ReviewDto>> Create(CreateReviewDto dto);
    Task<ApiResponse<List<ReviewDto>>> GetByProduct(int productId);
    Task<ApiResponse<List<ReviewDto>>> GetAll();
    Task<ApiResponse> Delete(int id);
}
