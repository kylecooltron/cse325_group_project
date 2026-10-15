using BlazorGroupProjectApp.Data;
using BlazorGroupProjectApp.Models;

namespace BlazorGroupProjectApp.Services;

public class ScoringService
{
    // Maximum "reasonable" deviation per field — predictions beyond these are treated as 100% bold
    private const double MaxTempDiff = 30.0;   // °F
    private const double MaxPrecipDiff = 2.0;  // inches
    private const double MaxWindDiff = 30.0;   // mph

    // Minimum deviation required to submit — must disagree with forecast by at least this much on at least one field
    private const double MinTempDiff = 1.0;
    private const double MinPrecipDiff = 0.05;
    private const double MinWindDiff = 1.0;

    private readonly ClimateSeerContext _db;

    public ScoringService(ClimateSeerContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Returns true if the prediction disagrees with the forecast enough to be worth submitting.
    /// </summary>
    public bool IsWorthSubmitting(Prediction prediction)
    {
        return
            Math.Abs(prediction.PredictedTempHigh - prediction.ForecastTempHigh) >= MinTempDiff ||
            Math.Abs(prediction.PredictedTempLow - prediction.ForecastTempLow) >= MinTempDiff ||
            Math.Abs(prediction.PredictedPrecipitation - prediction.ForecastPrecipitation) >= MinPrecipDiff ||
            Math.Abs(prediction.PredictedMaxWindSpeed - prediction.ForecastMaxWindSpeed) >= MinWindDiff;
    }

    /// <summary>
    /// Calculates and stores PotentialPoints on the prediction at submission time.
    /// Call this server-side before saving — never trust a client-supplied value.
    /// </summary>
    public void SetPotentialPoints(Prediction prediction)
    {
        var daysOut = (prediction.TargetDate.Date - DateTime.UtcNow.Date).TotalDays;

        // How bold is each field? (0–1 linear)
        var avgBoldness =
            (Boldness(prediction.PredictedTempHigh, prediction.ForecastTempHigh, MaxTempDiff) +
             Boldness(prediction.PredictedTempLow,  prediction.ForecastTempLow,  MaxTempDiff) +
             Boldness(prediction.PredictedPrecipitation, prediction.ForecastPrecipitation, MaxPrecipDiff) +
             Boldness(prediction.PredictedMaxWindSpeed,  prediction.ForecastMaxWindSpeed,  MaxWindDiff)) / 4.0;

        // 50 base points + 2 bonus points per day out, scaled by boldness
        var basePoints = 50 + (int)(daysOut * 2);
        prediction.PotentialPoints = (int)Math.Round(avgBoldness * basePoints);
    }

    /// <summary>
    /// Evaluates a prediction against the actual observed weather and returns the result.
    /// </summary>
    public PredictionResult Score(Prediction prediction, WeatherObservation actual)
    {
        // How well did the actual weather validate each field of your prediction vs the original forecast?
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

    // How bold is this prediction vs the forecast? Linear: 0 = same as forecast, 1 = max disagreement.
    private static double Boldness(double predicted, double forecast, double maxDiff)
    {
        var diff = Math.Abs(predicted - forecast);
        return Math.Min(diff / maxDiff, 1.0);
    }

    // What fraction of potential points does this field earn?
    // 1.0 = actual matched your prediction exactly; 0.0 = actual matched the forecast.
    private static double EarningRate(double predicted, double forecast, double actual)
    {
        var totalDisagreement = Math.Abs(predicted - forecast);
        if (totalDisagreement < 1e-9) return 0.0;

        // How far did actual move from the forecast toward your prediction? (clamped 0–1)
        var alignment = (actual - forecast) / (predicted - forecast);
        return Math.Max(0.0, Math.Min(1.0, alignment));
    }
}
