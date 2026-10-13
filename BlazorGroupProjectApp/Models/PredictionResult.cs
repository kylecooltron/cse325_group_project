namespace BlazorGroupProjectApp.Models;

public class PredictionResult
{
    public int Id { get; set; }
    public int PredictionId { get; set; }
    public double ActualTempHigh { get; set; }
    public double ActualTempLow { get; set; }
    public double ActualPrecipitation { get; set; }
    public double ActualMaxWindSpeed { get; set; }
    public double TempHighAccuracy { get; set; }
    public double TempLowAccuracy { get; set; }
    public double PrecipitationAccuracy { get; set; }
    public double WindAccuracy { get; set; }
    public int Points { get; set; }
    public DateTime EvaluatedAt { get; set; } = DateTime.UtcNow;

    public Prediction Prediction { get; set; } = null!;
}
