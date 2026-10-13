using BlazorGroupProjectApp.Data;
using BlazorGroupProjectApp.Models;
using Microsoft.AspNetCore.Identity;
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
            .GroupBy(p => p.UserId)
            .Select(g => new
            {
                UserId = g.Key,
                Points = g.Sum(p => p.Result!.Points)
            })
            .OrderByDescending(x => x.Points)
            .ToListAsync();

        var userIds = scores.Select(s => s.UserId).ToList();
        var users = await _db.Users
            .Where(u => userIds.Contains(u.Id))
            .Select(u => new { u.Id, u.Email })
            .ToListAsync();

        var emailById = users.ToDictionary(u => u.Id, u => u.Email ?? u.Id);

        return scores.Select((s, i) => new LeaderboardEntry
        {
            UserId = s.UserId,
            DisplayName = emailById.GetValueOrDefault(s.UserId, s.UserId),
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
