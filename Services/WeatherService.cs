using SmartPOS.Shared.Common;
using SmartPOS.Shared.DTOs.Weather;
using SmartPOS.Shared.Interfaces;

namespace SmartPOS.Services.MaryamJ
{
    public class WeatherService : IWeatherService
    {
        public Task<ApiResponse<WeatherDto>> GetCurrentWeather(string city)
        {
            throw new NotImplementedException();
        }
    }
}
