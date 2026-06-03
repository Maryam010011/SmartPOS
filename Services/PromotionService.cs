using Microsoft.EntityFrameworkCore;
using SmartPOS.Shared.Common;
using SmartPOS.Shared.DTOs.Promotions;
using SmartPOS.Shared.Enums;
using SmartPOS.Shared.Interfaces;
using SmartPOS.Web.Data;
using SmartPOS.Web.Models;

namespace SmartPOS.Services.MaryamJ;

public class PromotionService : IPromotionService
{
    private readonly AppDbContext _context;
    private static readonly Random _random = new();

    public PromotionService(AppDbContext context)
    {
        _context = context;
    }

    // ── Shahzain's Sales module compatibility methods ──

    public async Task<ApiResponse<PromotionDto>> GetByCode(string code)
    {
        try
        {
            var promotion = await _context.Promotions
                .FirstOrDefaultAsync(p => p.Code.ToLower() == code.Trim().ToLower());

            if (promotion == null)
                return ApiResponse<PromotionDto>.Fail("Promotion not found.");

            return ApiResponse<PromotionDto>.Ok(MapToDto(promotion));
        }
        catch (Exception ex)
        {
            return ApiResponse<PromotionDto>.Fail($"Error: {ex.Message}");
        }
    }

    public async Task<ApiResponse<ApplyPromoResultDto>> ValidateAndApply(string code, decimal orderTotal)
    {
        var result = await ValidatePromoCode(code, orderTotal);
        return new ApiResponse<ApplyPromoResultDto>
        {
            Success = result.Data?.IsValid ?? false,
            Message = result.Data?.IsValid == true ? "Valid promo code" : result.Data?.ErrorMessage ?? result.Message,
            Data = result.Data?.IsValid == true
                ? new ApplyPromoResultDto { IsValid = true, DiscountAmount = result.Data.DiscountAmount, Message = "Valid promo code" }
                : new ApplyPromoResultDto { IsValid = false, DiscountAmount = 0, Message = result.Data?.ErrorMessage ?? result.Message }
        };
    }

    public async Task<ApiResponse> IncrementUsage(int promoId)
    {
        return await ApplyPromoCode(promoId);
    }

    public async Task<ApiResponse<List<PromotionDto>>> GetAll()
    {
        return await GetAllPromotions();
    }

    public async Task<ApiResponse<PromotionDto>> Create(PromotionDto dto)
    {
        var createDto = new CreatePromotionDto
        {
            Code = dto.Code,
            DiscountType = dto.DiscountType,
            Value = dto.Value,
            MinOrderValue = dto.MinOrderValue,
            MaxUsageLimit = dto.MaxUsageLimit,
            ValidFrom = dto.ValidFrom,
            ValidTo = dto.ValidTo,
            IsActive = dto.IsActive,
            Description = dto.Description
        };
        return await CreatePromotion(createDto);
    }

    public async Task<ApiResponse<PromotionDto>> Update(PromotionDto dto)
    {
        var updateDto = new UpdatePromotionDto
        {
            Code = dto.Code,
            DiscountType = dto.DiscountType,
            Value = dto.Value,
            MinOrderValue = dto.MinOrderValue,
            MaxUsageLimit = dto.MaxUsageLimit,
            ValidFrom = dto.ValidFrom,
            ValidTo = dto.ValidTo,
            IsActive = dto.IsActive,
            Description = dto.Description
        };
        return await UpdatePromotion(dto.Id, updateDto);
    }

    public async Task<ApiResponse> Delete(int id)
    {
        return await DeletePromotion(id);
    }

    // ── MaryamJ's Promotion module methods ──

    public async Task<ApiResponse<List<PromotionDto>>> GetAllPromotions()
    {
        try
        {
            var promotions = await _context.Promotions
                .OrderByDescending(p => p.CreatedAt)
                .Select(p => MapToDto(p))
                .ToListAsync();

            return ApiResponse<List<PromotionDto>>.Ok(promotions);
        }
        catch (Exception ex)
        {
            return ApiResponse<List<PromotionDto>>.Fail($"Failed to retrieve promotions: {ex.Message}");
        }
    }

    public async Task<ApiResponse<PromotionDto>> GetById(int id)
    {
        try
        {
            var promotion = await _context.Promotions.FindAsync(id);
            if (promotion == null)
                return ApiResponse<PromotionDto>.Fail("Promotion not found.");

            return ApiResponse<PromotionDto>.Ok(MapToDto(promotion));
        }
        catch (Exception ex)
        {
            return ApiResponse<PromotionDto>.Fail($"Error: {ex.Message}");
        }
    }

    public async Task<ApiResponse<PromotionDto>> CreatePromotion(CreatePromotionDto dto)
    {
        try
        {
            if (dto.Value <= 0)
                return ApiResponse<PromotionDto>.Fail("Discount value must be greater than 0.");

            if (dto.MaxUsageLimit <= 0)
                return ApiResponse<PromotionDto>.Fail("Max usage limit must be greater than 0.");

            if (dto.ValidTo <= dto.ValidFrom)
                return ApiResponse<PromotionDto>.Fail("ValidTo date must be after ValidFrom date.");

            string code;
            if (string.IsNullOrWhiteSpace(dto.Code))
            {
                code = GeneratePromoCode();
            }
            else
            {
                code = dto.Code.Trim().ToUpper();
                var codeExists = await _context.Promotions
                    .AnyAsync(p => p.Code.ToLower() == code.ToLower());
                if (codeExists)
                    return ApiResponse<PromotionDto>.Fail("Promotion code already exists.");
            }

            var promotion = new Promotion
            {
                Code = code,
                DiscountType = dto.DiscountType,
                Value = dto.Value,
                MinOrderValue = dto.MinOrderValue,
                MaxUsageLimit = dto.MaxUsageLimit,
                UsageCount = 0,
                ValidFrom = dto.ValidFrom,
                ValidTo = dto.ValidTo,
                IsActive = dto.IsActive,
                Description = dto.Description?.Trim(),
                CreatedAt = DateTime.UtcNow
            };

            if (dto.DiscountType == DiscountType.Percentage && dto.Value > 100)
                return ApiResponse<PromotionDto>.Fail("Percentage discount cannot exceed 100%.");

            _context.Promotions.Add(promotion);
            await _context.SaveChangesAsync();

            return ApiResponse<PromotionDto>.Ok(MapToDto(promotion), "Promotion created successfully.");
        }
        catch (Exception ex)
        {
            return ApiResponse<PromotionDto>.Fail($"Failed to create promotion: {ex.Message}");
        }
    }

    public async Task<ApiResponse<PromotionDto>> UpdatePromotion(int id, UpdatePromotionDto dto)
    {
        try
        {
            var promotion = await _context.Promotions.FindAsync(id);
            if (promotion == null)
                return ApiResponse<PromotionDto>.Fail("Promotion not found.");

            if (!string.IsNullOrWhiteSpace(dto.Code))
            {
                var newCode = dto.Code.Trim().ToUpper();
                if (newCode.ToLower() != promotion.Code.ToLower())
                {
                    var codeExists = await _context.Promotions
                        .AnyAsync(p => p.Code.ToLower() == newCode.ToLower() && p.Id != id);
                    if (codeExists)
                        return ApiResponse<PromotionDto>.Fail("Promotion code already exists.");
                }
                promotion.Code = newCode;
            }

            if (dto.DiscountType.HasValue)
                promotion.DiscountType = dto.DiscountType.Value;

            if (dto.Value.HasValue)
            {
                if (dto.Value <= 0)
                    return ApiResponse<PromotionDto>.Fail("Discount value must be greater than 0.");
                if (promotion.DiscountType == DiscountType.Percentage && dto.Value > 100)
                    return ApiResponse<PromotionDto>.Fail("Percentage discount cannot exceed 100%.");
                promotion.Value = dto.Value.Value;
            }

            if (dto.MinOrderValue.HasValue)
                promotion.MinOrderValue = dto.MinOrderValue.Value;

            if (dto.MaxUsageLimit.HasValue)
            {
                if (dto.MaxUsageLimit <= 0)
                    return ApiResponse<PromotionDto>.Fail("Max usage limit must be greater than 0.");
                promotion.MaxUsageLimit = dto.MaxUsageLimit.Value;
            }

            if (dto.ValidFrom.HasValue && dto.ValidTo.HasValue)
            {
                if (dto.ValidTo <= dto.ValidFrom)
                    return ApiResponse<PromotionDto>.Fail("ValidTo date must be after ValidFrom date.");
                promotion.ValidFrom = dto.ValidFrom.Value;
                promotion.ValidTo = dto.ValidTo.Value;
            }

            if (dto.IsActive.HasValue)
                promotion.IsActive = dto.IsActive.Value;

            if (dto.Description != null)
                promotion.Description = dto.Description?.Trim();

            await _context.SaveChangesAsync();
            return ApiResponse<PromotionDto>.Ok(MapToDto(promotion), "Promotion updated successfully.");
        }
        catch (Exception ex)
        {
            return ApiResponse<PromotionDto>.Fail($"Failed to update promotion: {ex.Message}");
        }
    }

    public async Task<ApiResponse> DeletePromotion(int id)
    {
        try
        {
            var promotion = await _context.Promotions.FindAsync(id);
            if (promotion == null)
                return ApiResponse.Fail("Promotion not found.");

            _context.Promotions.Remove(promotion);
            await _context.SaveChangesAsync();
            return ApiResponse.Ok("Promotion deleted successfully.");
        }
        catch (Exception ex)
        {
            return ApiResponse.Fail($"Failed to delete promotion: {ex.Message}");
        }
    }

    public async Task<ApiResponse> ToggleActive(int id)
    {
        try
        {
            var promotion = await _context.Promotions.FindAsync(id);
            if (promotion == null)
                return ApiResponse.Fail("Promotion not found.");

            promotion.IsActive = !promotion.IsActive;
            await _context.SaveChangesAsync();
            return ApiResponse.Ok($"Promotion {(promotion.IsActive ? "activated" : "deactivated")} successfully.");
        }
        catch (Exception ex)
        {
            return ApiResponse.Fail($"Failed to toggle promotion status: {ex.Message}");
        }
    }

    public async Task<ApiResponse<PromoValidationResult>> ValidatePromoCode(string code, decimal orderTotal)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(code))
                return ApiResponse<PromoValidationResult>.Ok(new PromoValidationResult
                {
                    IsValid = false,
                    ErrorMessage = "Promo code is required."
                });

            var promotion = await _context.Promotions
                .FirstOrDefaultAsync(p => p.Code.ToLower() == code.Trim().ToLower());

            if (promotion == null)
                return ApiResponse<PromoValidationResult>.Ok(new PromoValidationResult
                {
                    IsValid = false,
                    ErrorMessage = "Invalid promo code."
                });

            if (!promotion.IsActive)
                return ApiResponse<PromoValidationResult>.Ok(new PromoValidationResult
                {
                    IsValid = false,
                    ErrorMessage = "This promo code is currently inactive."
                });

            var now = DateTime.UtcNow;
            if (now < promotion.ValidFrom)
                return ApiResponse<PromoValidationResult>.Ok(new PromoValidationResult
                {
                    IsValid = false,
                    ErrorMessage = "This promo code is not yet valid."
                });

            if (now > promotion.ValidTo)
                return ApiResponse<PromoValidationResult>.Ok(new PromoValidationResult
                {
                    IsValid = false,
                    ErrorMessage = "This promo code has expired."
                });

            if (promotion.UsageCount >= promotion.MaxUsageLimit)
                return ApiResponse<PromoValidationResult>.Ok(new PromoValidationResult
                {
                    IsValid = false,
                    ErrorMessage = "This promo code has reached its maximum usage limit."
                });

            if (orderTotal < promotion.MinOrderValue)
                return ApiResponse<PromoValidationResult>.Ok(new PromoValidationResult
                {
                    IsValid = false,
                    ErrorMessage = $"Minimum order value of Rs {promotion.MinOrderValue:N0} required for this promo."
                });

            decimal discountAmount;
            if (promotion.DiscountType == DiscountType.Percentage)
            {
                discountAmount = orderTotal * (promotion.Value / 100);
            }
            else
            {
                discountAmount = promotion.Value;
                if (discountAmount > orderTotal)
                    discountAmount = orderTotal;
            }

            return ApiResponse<PromoValidationResult>.Ok(new PromoValidationResult
            {
                IsValid = true,
                DiscountAmount = discountAmount,
                DiscountType = promotion.DiscountType,
                PromotionId = promotion.Id,
                ErrorMessage = string.Empty
            });
        }
        catch (Exception ex)
        {
            return ApiResponse<PromoValidationResult>.Fail($"Error validating promo code: {ex.Message}");
        }
    }

    public async Task<ApiResponse> ApplyPromoCode(int promoId)
    {
        try
        {
            var promotion = await _context.Promotions.FindAsync(promoId);
            if (promotion == null)
                return ApiResponse.Fail("Promotion not found.");

            if (promotion.UsageCount >= promotion.MaxUsageLimit)
                return ApiResponse.Fail("Promotion has already reached maximum usage.");

            promotion.UsageCount++;
            await _context.SaveChangesAsync();
            return ApiResponse.Ok("Promotion usage recorded.");
        }
        catch (Exception ex)
        {
            return ApiResponse.Fail($"Failed to apply promo code: {ex.Message}");
        }
    }

    public async Task<ApiResponse<PromoAnalyticsDto>> GetPromoAnalytics(int id)
    {
        try
        {
            var promotion = await _context.Promotions.FindAsync(id);
            if (promotion == null)
                return ApiResponse<PromoAnalyticsDto>.Fail("Promotion not found.");

            var salesWithPromo = await _context.Sales
                .Where(s => s.PromoId == id)
                .ToListAsync();

            var totalTimesUsed = salesWithPromo.Count;
            var totalDiscountGiven = salesWithPromo.Sum(s => s.DiscountAmount);
            var revenueFromPromoOrders = salesWithPromo.Sum(s => s.TotalAmount);

            var dailyUsage = salesWithPromo
                .GroupBy(s => s.SaleDate.Date)
                .Select(g => new DailyUsageStat
                {
                    Date = g.Key,
                    UsageCount = g.Count(),
                    TotalDiscount = g.Sum(s => s.DiscountAmount),
                    TotalRevenue = g.Sum(s => s.TotalAmount)
                })
                .OrderBy(d => d.Date)
                .ToList();

            var analytics = new PromoAnalyticsDto
            {
                TotalTimesUsed = totalTimesUsed,
                TotalDiscountGiven = totalDiscountGiven,
                RevenueFromPromoOrders = revenueFromPromoOrders,
                DailyUsageList = dailyUsage
            };

            return ApiResponse<PromoAnalyticsDto>.Ok(analytics);
        }
        catch (Exception ex)
        {
             return ApiResponse<PromoAnalyticsDto>.Fail($"Failed to get analytics: {ex.Message}");
         }
     }

     public async Task<ApiResponse<List<PromotionDto>>> GetActivePromotions()
     {
         try
         {
             var now = DateTime.UtcNow;
             var activePromotions = await _context.Promotions
                 .Where(p => p.IsActive && now >= p.ValidFrom && now <= p.ValidTo && p.UsageCount < p.MaxUsageLimit)
                 .OrderBy(p => p.ValidTo)
                 .ToListAsync();

             var dtos = activePromotions.Select(MapToDto).ToList();
             return ApiResponse<List<PromotionDto>>.Ok(dtos);
         }
         catch (Exception ex)
         {
             return ApiResponse<List<PromotionDto>>.Fail($"Failed to get active promotions: {ex.Message}");
         }
     }

     // ── Helpers ──

    private static string GeneratePromoCode()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var randomPart = new string(Enumerable.Repeat(chars, 6)
            .Select(s => s[_random.Next(s.Length)]).ToArray());
        return $"PROMO-{randomPart}";
    }

    private static PromotionDto MapToDto(Promotion p)
    {
        return new PromotionDto
        {
            Id = p.Id,
            Code = p.Code,
            DiscountType = p.DiscountType,
            Value = p.Value,
            MinOrderValue = p.MinOrderValue,
            MaxUsageLimit = p.MaxUsageLimit,
            UsageCount = p.UsageCount,
            ValidFrom = p.ValidFrom,
            ValidTo = p.ValidTo,
            IsActive = p.IsActive,
            Description = p.Description,
            CreatedAt = p.CreatedAt
        };
    }
}
