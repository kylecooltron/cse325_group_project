using BlazorGroupProjectApp.Models;

namespace BlazorGroupProjectApp.Services;

public class WeatherService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<WeatherService> _logger;

    public WeatherService(HttpClient httpClient, ILogger<WeatherService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<WeatherObservation?> GetCurrentWeatherAsync(string location)
    {
        // TODO: integrate real weather API (e.g. Open-Meteo, OpenWeatherMap)
        await Task.Delay(0);
        return new WeatherObservation
        {
            Location = location,
            ObservedAt = DateTime.UtcNow,
            Temperature = 72,
            Precipitation = 0,
            WindSpeed = 5,
            Description = "Sunny"
        };
    }

    public async Task<WeatherObservation?> GetHistoricalWeatherAsync(string location, DateTime date)
    {
        // TODO: fetch actual historical weather for scoring
        await Task.Delay(0);
        return null;
    }
}
