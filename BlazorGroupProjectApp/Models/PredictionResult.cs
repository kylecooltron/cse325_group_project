namespace BlazorGroupProjectApp.Models;

public class PredictionResult
{
    public int Id { get; set; }
    public int PredictionId { get; set; }
    public double ActualTemperature { get; set; }
    public double ActualPrecipitation { get; set; }
    public double ActualWindSpeed { get; set; }
    public double TemperatureAccuracy { get; set; }
    public double PrecipitationAccuracy { get; set; }
    public double WindAccuracy { get; set; }
    public int Points { get; set; }
    public DateTime EvaluatedAt { get; set; } = DateTime.UtcNow;

    public Prediction Prediction { get; set; } = null!;
}
