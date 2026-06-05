using SmartPOS.Shared.Common;
using SmartPOS.Shared.DTOs.Promotions;

namespace SmartPOS.Shared.Interfaces;

public interface IPromotionService
{
    Task<ApiResponse<List<PromotionDto>>> GetAllPromotions();
    Task<ApiResponse<PromotionDto>> GetById(int id);
    Task<ApiResponse<PromotionDto>> GetByCode(string code);
    Task<ApiResponse<PromotionDto>> CreatePromotion(CreatePromotionDto dto);
    Task<ApiResponse<PromotionDto>> UpdatePromotion(int id, UpdatePromotionDto dto);
    Task<ApiResponse> DeletePromotion(int id);
    Task<ApiResponse> ToggleActive(int id);
    Task<ApiResponse<PromoValidationResult>> ValidatePromoCode(string code, decimal orderTotal);
    Task<ApiResponse<PromoAnalyticsDto>> GetPromoAnalytics(int id);
    Task<ApiResponse<List<PromotionDto>>> GetActivePromotions();
}
