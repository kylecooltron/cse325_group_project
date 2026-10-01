namespace BlazorGroupProjectApp.Models;

public class LeaderboardEntry
{
    public int UserProfileId { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public int Year { get; set; }
    public int Month { get; set; }
    public int Points { get; set; }
    public int Rank { get; set; }
}
