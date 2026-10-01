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
        var startDate = new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Utc);
        var endDate = startDate.AddMonths(1);

        var scores = await _db.Predictions
            .Where(p => p.TargetDate >= startDate && p.TargetDate < endDate && p.Result != null)
            .Include(p => p.Result)
            .Include(p => p.UserProfile)
            .GroupBy(p => new { p.UserProfileId, p.UserProfile.DisplayName })
            .Select(g => new
            {
                g.Key.UserProfileId,
                g.Key.DisplayName,
                Points = g.Sum(p => p.Result!.Points)
            })
            .OrderByDescending(x => x.Points)
            .ToListAsync();

        return scores.Select((s, i) => new LeaderboardEntry
        {
            UserProfileId = s.UserProfileId,
            DisplayName = s.DisplayName,
            Year = year,
            Month = month,
            Points = s.Points,
            Rank = i + 1
        }).ToList();
    }

    public async Task<List<LeaderboardEntry>> GetCurrentMonthLeaderboardAsync()
    {
        var now = DateTime.UtcNow;
        return await GetMonthlyLeaderboardAsync(now.Year, now.Month);
    }
}
