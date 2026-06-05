using SmartPOS.Shared.Enums;

namespace SmartPOS.Shared.DTOs.Promotions;

public class PromoValidationResult
{
    public bool IsValid { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
    public decimal DiscountAmount { get; set; }
    public DiscountType DiscountType { get; set; }
    public int PromotionId { get; set; }
}
