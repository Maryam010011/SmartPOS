namespace SmartPOS.Shared.DTOs.Promotions;

public class ApplyPromoResultDto
{
    public bool IsValid { get; set; }
    public decimal DiscountAmount { get; set; }
    public string Message { get; set; } = string.Empty;
}
