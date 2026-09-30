using BlazorGroupProjectApp.Data;
using BlazorGroupProjectApp.Models;
using Microsoft.EntityFrameworkCore;

namespace BlazorGroupProjectApp.Services;

public class LeaderboardService
{
    private readonly ClimateSeerContext _db;

    public LeaderboardService(ClimateSeerContext db)
    {
        _db = db;
    }

    public async Task<List<LeaderboardEntry>> GetMonthlyLeaderboardAsync(int year, int month)
    {
        // TODO: calculate from scored predictions for the given month
        return await _db.LeaderboardEntries
            .Where(e => e.Year == year && e.Month == month)
            .Include(e => e.UserProfile)
            .OrderBy(e => e.Rank)
            .ToListAsync();
    }

    public async Task<List<LeaderboardEntry>> GetCurrentMonthLeaderboardAsync()
    {
        var now = DateTime.UtcNow;
        return await GetMonthlyLeaderboardAsync(now.Year, now.Month);
    }

    public async Task RecalculateMonthlyLeaderboardAsync(int year, int month)
    {
        // TODO: aggregate scored predictions and upsert leaderboard entries
        await Task.CompletedTask;
    }
}
