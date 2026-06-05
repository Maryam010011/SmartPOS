using Microsoft.AspNetCore.Mvc;
using SmartPOS.Shared.Interfaces;

namespace SmartPOS.Web.Controllers
{
    /// <summary>
    /// API controller for retrieving current weather data.
    /// Provides an endpoint to fetch weather information for a given city
    /// via the OpenWeatherMap API integration.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class WeatherController : ControllerBase
    {
        private readonly IWeatherService _weatherService;

        /// <summary>
        /// Initializes a new instance of the <see cref="WeatherController"/> class.
        /// </summary>
        /// <param name="weatherService">The weather service instance injected via DI.</param>
        public WeatherController(IWeatherService weatherService)
        {
            _weatherService = weatherService;
        }

        /// <summary>
        /// Retrieves the current weather data for the specified city.
        /// </summary>
        /// <param name="city">The city name to fetch weather data for (e.g. "Islamabad").</param>
        /// <returns>An ApiResponse containing the current weather condition, temperature, and city name.</returns>
        /// <response code="200">Returns the current weather data.</response>
        [HttpGet]
        public async Task<IActionResult> GetCurrentWeather([FromQuery] string city)
        {
            var result = await _weatherService.GetCurrentWeather(city);
            return Ok(result);
        }
    }
}
