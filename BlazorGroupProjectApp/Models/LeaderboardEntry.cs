namespace BlazorGroupProjectApp.Models;

public class LeaderboardEntry
{
    public int Id { get; set; }
    public int UserProfileId { get; set; }
    public int Year { get; set; }
    public int Month { get; set; }
    public int Points { get; set; }
    public int Rank { get; set; }

    public UserProfile UserProfile { get; set; } = null!;
}
