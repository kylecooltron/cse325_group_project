using BlazorGroupProjectApp.Models;
using BlazorGroupProjectApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace BlazorGroupProjectApp.Controllers;

[Route("api/weather")]
[ApiController]
public class WeatherController : Controller
{
    private readonly WeatherService _weatherService;

    public WeatherController(WeatherService weatherService)
    {
        _weatherService = weatherService;
    }

    [HttpGet("current")]
    public async Task<ActionResult<WeatherObservation>> GetCurrent([FromQuery] string location)
    {
        if (string.IsNullOrWhiteSpace(location))
            return BadRequest("Location is required.");

        var weather = await _weatherService.GetCurrentWeatherAsync(location);
        if (weather == null) return NotFound();
        return Ok(weather);
    }

    [HttpGet("historical")]
    public async Task<ActionResult<WeatherObservation>> GetHistorical(
        [FromQuery] string location,
        [FromQuery] DateTime date)
    {
        if (string.IsNullOrWhiteSpace(location))
            return BadRequest("Location is required.");

        var weather = await _weatherService.GetHistoricalWeatherAsync(location, date);
        if (weather == null) return NotFound();
        return Ok(weather);
    }
}
