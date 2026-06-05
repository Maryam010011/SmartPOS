namespace SmartPOS.Shared.DTOs.Weather;

public class WeatherDto
{
    public string City { get; set; } = string.Empty;
    public string Condition { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public double Temperature { get; set; }
    public double FeelsLike { get; set; }
    public int Humidity { get; set; }
    public double WindSpeed { get; set; }
    public string Icon { get; set; } = string.Empty;
    public DateTime FetchedAt { get; set; }
}
