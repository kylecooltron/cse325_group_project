using System.Text.Json;
using System.Text.Json.Serialization;
using BlazorGroupProjectApp.Models;

namespace BlazorGroupProjectApp.Services;

public class WeatherService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<WeatherService> _logger;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public WeatherService(HttpClient httpClient, ILogger<WeatherService> logger)
        => (_httpClient, _logger) = (httpClient, logger);

    public async Task<WeatherObservation?> GetForecastAsync(string location, DateTime date)
    {
        var coords = await GeocodeAsync(location);
        if (coords == null) return null;
        return await GetForecastAsync(coords.Value.Lat, coords.Value.Lon, date);
    }

    public async Task<WeatherObservation?> GetForecastAsync(double lat, double lon, DateTime date)
    {
        var dateStr = date.ToString("yyyy-MM-dd");
        // Use the archive API for past dates, forecast API for future dates
        var baseUrl = date.Date < DateTime.UtcNow.Date
            ? "https://archive-api.open-meteo.com/v1/archive"
            : "https://api.open-meteo.com/v1/forecast";

        var url = $"{baseUrl}?latitude={lat}&longitude={lon}" +
                  $"&daily=temperature_2m_max,temperature_2m_min,precipitation_sum,wind_speed_10m_max,weather_code" +
                  $"&temperature_unit=fahrenheit&wind_speed_unit=mph&precipitation_unit=inch" +
                  $"&start_date={dateStr}&end_date={dateStr}&timezone=auto";

        try
        {
            var response = await _httpClient.GetFromJsonAsync<OpenMeteoForecastResponse>(url, JsonOptions);
            return ParseDailyResponse(response, date, null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch forecast for ({Lat},{Lon}) on {Date}", lat, lon, date);
            return null;
        }
    }

    public async Task<WeatherObservation?> GetCurrentWeatherAsync(string location)
    {
        var coords = await GeocodeAsync(location);
        if (coords == null) return null;

        var url = $"https://api.open-meteo.com/v1/forecast" +
                  $"?latitude={coords.Value.Lat}&longitude={coords.Value.Lon}" +
                  $"&daily=temperature_2m_max,temperature_2m_min,precipitation_sum,wind_speed_10m_max,weather_code" +
                  $"&temperature_unit=fahrenheit&wind_speed_unit=mph&precipitation_unit=inch" +
                  $"&forecast_days=1&timezone=auto";

        try
        {
            var response = await _httpClient.GetFromJsonAsync<OpenMeteoForecastResponse>(url, JsonOptions);
            return ParseDailyResponse(response, DateTime.UtcNow, location);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch current weather for {Location}", location);
            return null;
        }
    }

    public async Task<WeatherObservation?> GetHistoricalWeatherAsync(string location, DateTime date)
    {
        var coords = await GeocodeAsync(location);
        if (coords == null) return null;

        var dateStr = date.ToString("yyyy-MM-dd");
        var url = $"https://archive-api.open-meteo.com/v1/archive" +
                  $"?latitude={coords.Value.Lat}&longitude={coords.Value.Lon}" +
                  $"&daily=temperature_2m_max,temperature_2m_min,precipitation_sum,wind_speed_10m_max,weather_code" +
                  $"&temperature_unit=fahrenheit&wind_speed_unit=mph&precipitation_unit=inch" +
                  $"&start_date={dateStr}&end_date={dateStr}&timezone=auto";

        try
        {
            var response = await _httpClient.GetFromJsonAsync<OpenMeteoForecastResponse>(url, JsonOptions);
            return ParseDailyResponse(response, date, location);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch historical weather for {Location} on {Date}", location, date);
            return null;
        }
    }

    private static WeatherObservation? ParseDailyResponse(OpenMeteoForecastResponse? response, DateTime observedAt, string? location)
    {
        if (response?.Daily == null) return null;

        var d = response.Daily;
        var code = (int)(d.WeatherCode?.FirstOrDefault() ?? 0);
        return new WeatherObservation
        {
            Location = location ?? string.Empty,
            ObservedAt = observedAt,
            TempHigh = Math.Round(d.Temperature2mMax?.FirstOrDefault() ?? 0, 1),
            TempLow = Math.Round(d.Temperature2mMin?.FirstOrDefault() ?? 0, 1),
            Precipitation = Math.Round(d.PrecipitationSum?.FirstOrDefault() ?? 0, 2),
            MaxWindSpeed = Math.Round(d.WindSpeed10mMax?.FirstOrDefault() ?? 0, 1),
            Description = WmoCodeToDescription(code)
        };
    }

    private async Task<(double Lat, double Lon)?> GeocodeAsync(string location)
    {
        var url = $"https://geocoding-api.open-meteo.com/v1/search?name={Uri.EscapeDataString(location)}&count=1";
        try
        {
            var response = await _httpClient.GetFromJsonAsync<GeocodingResponse>(url, JsonOptions);
            var result = response?.Results?.FirstOrDefault();
            if (result == null) return null;
            return (result.Latitude, result.Longitude);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Geocoding failed for {Location}", location);
            return null;
        }
    }

    private static string WmoCodeToDescription(int code) => code switch
    {
        0 => "Clear sky",
        1 => "Mainly clear",
        2 => "Partly cloudy",
        3 => "Overcast",
        45 or 48 => "Foggy",
        51 or 53 or 55 => "Drizzle",
        61 or 63 or 65 => "Rain",
        71 or 73 or 75 => "Snow",
        80 or 81 or 82 => "Rain showers",
        85 or 86 => "Snow showers",
        95 => "Thunderstorm",
        96 or 99 => "Thunderstorm with hail",
        _ => "Unknown"
    };

    // Models that match the Open-Meteo API response structure
    private class OpenMeteoForecastResponse
    {
        public DailyData? Daily { get; set; }
    }

    private class DailyData
    {
        [JsonPropertyName("temperature_2m_max")]
        public List<double>? Temperature2mMax { get; set; }

        [JsonPropertyName("temperature_2m_min")]
        public List<double>? Temperature2mMin { get; set; }

        [JsonPropertyName("precipitation_sum")]
        public List<double>? PrecipitationSum { get; set; }

        [JsonPropertyName("wind_speed_10m_max")]
        public List<double>? WindSpeed10mMax { get; set; }

        [JsonPropertyName("weather_code")]
        public List<double>? WeatherCode { get; set; }
    }

    private class GeocodingResponse
    {
        public List<GeocodingResult>? Results { get; set; }
    }

    private class GeocodingResult
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
