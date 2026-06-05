using SmartPOS.Shared.Enums;

namespace SmartPOS.Shared.DTOs.Promotions;

public class CreatePromotionDto
{
    public string Code { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DiscountType DiscountType { get; set; }
    public decimal Value { get; set; }
    public decimal MinOrderValue { get; set; }
    public int? MaxUsageLimit { get; set; }
    public DateOnly ValidFrom { get; set; }
    public DateOnly ValidTo { get; set; }
    public bool IsActive { get; set; }
}
