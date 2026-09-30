using BlazorGroupProjectApp.Models;
using BlazorGroupProjectApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace BlazorGroupProjectApp.Controllers;

[Route("api/leaderboard")]
[ApiController]
public class LeaderboardController : Controller
{
    private readonly LeaderboardService _leaderboardService;

    public LeaderboardController(LeaderboardService leaderboardService)
    {
        _leaderboardService = leaderboardService;
    }

    [HttpGet("current")]
    public async Task<ActionResult<List<LeaderboardEntry>>> GetCurrent()
    {
        var entries = await _leaderboardService.GetCurrentMonthLeaderboardAsync();
        return Ok(entries);
    }

    [HttpGet]
    public async Task<ActionResult<List<LeaderboardEntry>>> GetMonthly(
        [FromQuery] int year,
        [FromQuery] int month)
    {
        var entries = await _leaderboardService.GetMonthlyLeaderboardAsync(year, month);
        return Ok(entries);
    }
}
