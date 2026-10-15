using BlazorGroupProjectApp.Data;
using BlazorGroupProjectApp.Models;

namespace BlazorGroupProjectApp.Services;

public class ScoringService
{
    // Max deviation before a prediction is considered "fully bold" (0-1 scale)
    private const double MaxTempDiff = 30.0;   // °F
    private const double MaxPrecipDiff = 2.0;  // inches
    private const double MaxWindDiff = 30.0;   // mph

    // Minimum change from the forecast needed to submit
    private const double MinTempDiff = 1.0;
    private const double MinPrecipDiff = 0.05;
    private const double MinWindDiff = 1.0;

    private readonly ClimateSeerContext _db;

    public ScoringService(ClimateSeerContext db)
    {
        _db = db;
    }

    // Returns true if the prediction is different enough from the forecast to submit
    public bool IsWorthSubmitting(Prediction prediction)
    {
        return
            Math.Abs(prediction.PredictedTempHigh - prediction.ForecastTempHigh) >= MinTempDiff ||
            Math.Abs(prediction.PredictedTempLow - prediction.ForecastTempLow) >= MinTempDiff ||
            Math.Abs(prediction.PredictedPrecipitation - prediction.ForecastPrecipitation) >= MinPrecipDiff ||
            Math.Abs(prediction.PredictedMaxWindSpeed - prediction.ForecastMaxWindSpeed) >= MinWindDiff;
    }

    // Calculates how many points this prediction is worth and saves it to the prediction
    public void SetPotentialPoints(Prediction prediction)
    {
        var daysOut = (prediction.TargetDate.Date - DateTime.UtcNow.Date).TotalDays;

        // Average boldness across all four fields (0–1)
        var avgBoldness =
            (Boldness(prediction.PredictedTempHigh, prediction.ForecastTempHigh, MaxTempDiff) +
             Boldness(prediction.PredictedTempLow,  prediction.ForecastTempLow,  MaxTempDiff) +
             Boldness(prediction.PredictedPrecipitation, prediction.ForecastPrecipitation, MaxPrecipDiff) +
             Boldness(prediction.PredictedMaxWindSpeed,  prediction.ForecastMaxWindSpeed,  MaxWindDiff)) / 4.0;

        // 50 base points + 2 bonus points per day out, scaled by boldness
        var basePoints = 50 + (int)(daysOut * 2);
        prediction.PotentialPoints = (int)Math.Round(avgBoldness * basePoints);
    }

    // Scores a prediction against what actually happened and returns the result
    public PredictionResult Score(Prediction prediction, WeatherObservation actual)
    {
        var tempHighEarning  = EarningRate(prediction.PredictedTempHigh, prediction.ForecastTempHigh, actual.TempHigh);
        var tempLowEarning   = EarningRate(prediction.PredictedTempLow,  prediction.ForecastTempLow,  actual.TempLow);
        var precipEarning    = EarningRate(prediction.PredictedPrecipitation, prediction.ForecastPrecipitation, actual.Precipitation);
        var windEarning      = EarningRate(prediction.PredictedMaxWindSpeed,  prediction.ForecastMaxWindSpeed,  actual.MaxWindSpeed);

        var avgEarning = (tempHighEarning + tempLowEarning + precipEarning + windEarning) / 4.0;
        var pointsEarned = (int)Math.Round(prediction.PotentialPoints * avgEarning);

        return new PredictionResult
        {
            PredictionId = prediction.Id,
            ActualTempHigh = actual.TempHigh,
            ActualTempLow = actual.TempLow,
            ActualPrecipitation = actual.Precipitation,
            ActualMaxWindSpeed = actual.MaxWindSpeed,
            TempHighAccuracy = tempHighEarning,
            TempLowAccuracy = tempLowEarning,
            PrecipitationAccuracy = precipEarning,
            WindAccuracy = windEarning,
            Points = pointsEarned,
            EvaluatedAt = DateTime.UtcNow
        };
    }

    public async Task EvaluatePendingPredictionsAsync()
    {
        // TODO: query predictions past their target date without results and score them
        await Task.CompletedTask;
    }

    // Returns 0-1 representing how far the prediction deviates from the forecast
    private static double Boldness(double predicted, double forecast, double maxDiff)
    {
        var diff = Math.Abs(predicted - forecast);
        return Math.Min(diff / maxDiff, 1.0);
    }

    // Returns what fraction of potential points this field earns (0 = wrong, 1 = perfect)
    private static double EarningRate(double predicted, double forecast, double actual)
    {
        var totalDisagreement = Math.Abs(predicted - forecast);
        if (totalDisagreement < 1e-9) return 0.0;

        var alignment = (actual - forecast) / (predicted - forecast);
        return Math.Max(0.0, Math.Min(1.0, alignment));
    }
}
