using SmartPOS.Shared.Common;
using SmartPOS.Shared.DTOs.Weather;

namespace SmartPOS.Shared.Interfaces;

public interface IWeatherService
{
    Task<ApiResponse<WeatherDto>> GetCurrentWeather(string city);
}
