using SmartPOS.Shared.Common;
using SmartPOS.Shared.DTOs.Reviews;

namespace SmartPOS.Shared.Interfaces;

public interface IBERTService
{
    Task<ApiResponse<string>> AnalyzeSentiment(string text);
    Task<ApiResponse<List<SentimentStatDto>>> GetSentimentStats(int? productId = null);
}
