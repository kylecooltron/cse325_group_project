namespace BlazorGroupProjectApp.Models;

public class Prediction
{
    public int Id { get; set; }
    public int UserProfileId { get; set; }
    public string Location { get; set; } = string.Empty;
    public DateTime TargetDate { get; set; }
    public double PredictedTemperature { get; set; }
    public double PredictedPrecipitation { get; set; }
    public double PredictedWindSpeed { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime Deadline { get; set; }
    public PredictionStatus Status { get; set; } = PredictionStatus.Pending;

    public UserProfile UserProfile { get; set; } = null!;
    public PredictionResult? Result { get; set; }
}

public enum PredictionStatus
{
    Pending,
    Evaluated,
    Scored
}
