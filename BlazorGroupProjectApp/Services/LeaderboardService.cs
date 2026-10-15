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
            .AsNoTracking()
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
        var nameById = await GetDisplayNamesAsync(userIds);

        return scores.Select((s, i) => new LeaderboardEntry
        {
            UserId = s.UserId,
            DisplayName = nameById.GetValueOrDefault(s.UserId, s.UserId),
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

    public async Task<List<LeaderboardEntry>> GetYearLeaderboardAsync(int year)
    {
        var startDate = new DateTime(year, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var endDate = startDate.AddYears(1);

        var scores = await _db.Predictions
            .AsNoTracking()
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
        var nameById = await GetDisplayNamesAsync(userIds);

        return scores.Select((s, i) => new LeaderboardEntry
        {
            UserId = s.UserId,
            DisplayName = nameById.GetValueOrDefault(s.UserId, s.UserId),
            Year = year,
            Month = 0,
            Points = s.Points,
            Rank = i + 1
        }).ToList();
    }

    // Looks up the given name for each user from their stored claims, falls back to email
    private async Task<Dictionary<string, string>> GetDisplayNamesAsync(List<string> userIds)
    {
        var givenNameClaimType = System.Security.Claims.ClaimTypes.GivenName;

        var claims = await _db.UserClaims
            .Where(c => userIds.Contains(c.UserId) && c.ClaimType == givenNameClaimType)
            .Select(c => new { c.UserId, c.ClaimValue })
            .ToListAsync();

        var nameById = claims.ToDictionary(c => c.UserId, c => c.ClaimValue ?? c.UserId);

        // Fall back to email for any users missing the claim
        var missing = userIds.Where(id => !nameById.ContainsKey(id)).ToList();
        if (missing.Count > 0)
        {
            var emails = await _db.Users
                .Where(u => missing.Contains(u.Id))
                .Select(u => new { u.Id, u.Email })
                .ToListAsync();

            foreach (var u in emails)
                nameById[u.Id] = u.Email ?? u.Id;
        }

        return nameById;
    }
}
