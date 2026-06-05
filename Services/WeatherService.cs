using SmartPOS.Shared.Interfaces;
using SmartPOS.Shared.DTOs;
using SmartPOS.Shared.DTOs.Weather;
using SmartPOS.Shared.Common;
using System.Text.Json;

namespace SmartPOS.Web.Services
{
    /// <summary>
    /// Service for retrieving current weather data from the OpenWeatherMap API.
    /// Returns weather condition, temperature, and city information wrapped
    /// in an <see cref="ApiResponse{T}"/> for consistent error handling.
    /// </summary>
    public class WeatherService : IWeatherService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        /// <summary>
        /// Initializes a new instance of the <see cref="WeatherService"/> class.
        /// </summary>
        /// <param name="httpClient">The HTTP client for making OpenWeatherMap API requests.</param>
        /// <param name="configuration">The application configuration containing the WeatherService API key.</param>
        public WeatherService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["WeatherService:ApiKey"] ?? string.Empty;
        }

        /// <summary>
        /// Retrieves the current weather data for the specified city
        /// from the OpenWeatherMap API using metric units.
        /// </summary>
        /// <param name="city">The city name to fetch weather data for (e.g. "Islamabad").</param>
        /// <returns>
        /// An <see cref="ApiResponse{WeatherDto}"/> containing the weather condition,
        /// temperature in Celsius, city name, and fetch timestamp on success;
        /// or a failure response with an error message on any exception.
        /// </returns>
        public async Task<ApiResponse<WeatherDto>> GetCurrentWeather(string city)
        {
            try
            {
                var url = $"https://api.openweathermap.org/data/2.5/weather?q={city}&appid={_apiKey}&units=metric";

                var response = await _httpClient.GetAsync(url);
                var json = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                    return ApiResponse<WeatherDto>.Fail($"OpenWeatherMap API error ({response.StatusCode}): {json}");

                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                var condition = root.GetProperty("weather")[0].GetProperty("description").GetString() ?? string.Empty;
                var temperature = root.GetProperty("main").GetProperty("temp").GetDouble();
                var cityName = root.GetProperty("name").GetString() ?? city;

                var result = new WeatherDto
                {
                    Condition = condition,
                    Temperature = temperature,
                    City = cityName,
                    FetchedAt = DateTime.UtcNow
                };

                return ApiResponse<WeatherDto>.Ok(result, $"Weather data retrieved for {cityName}.");
            }
            catch (Exception ex)
            {
                return ApiResponse<WeatherDto>.Fail($"Error fetching weather data: {ex.Message}");
            }
        }
    }
}
