namespace BlazorGroupProjectApp.Models;

public class UserProfile
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty; // ASP.NET Core Identity user ID
    public string DisplayName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Prediction> Predictions { get; set; } = new List<Prediction>();
}
