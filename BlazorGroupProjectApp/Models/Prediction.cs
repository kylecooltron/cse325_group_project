namespace BlazorGroupProjectApp.Models;

public class Prediction
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public DateTime TargetDate { get; set; }
    public double PredictedTempHigh { get; set; }
    public double PredictedTempLow { get; set; }
    public double PredictedPrecipitation { get; set; }
    public double PredictedMaxWindSpeed { get; set; }

    // Forecast values at the time the prediction was submitted — used for scoring
    public double ForecastTempHigh { get; set; }
    public double ForecastTempLow { get; set; }
    public double ForecastPrecipitation { get; set; }
    public double ForecastMaxWindSpeed { get; set; }

    public int PotentialPoints { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime Deadline { get; set; }
    public PredictionStatus Status { get; set; } = PredictionStatus.Pending;

    public PredictionResult? Result { get; set; }
}

public enum PredictionStatus
{
    Pending,
    Evaluated,
    Scored
}
