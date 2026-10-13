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

        // Each field: how bold is this prediction? (0–1, exponential curve)
        var tempHighBoldness = Boldness(prediction.PredictedTempHigh, prediction.ForecastTempHigh, MaxTempDiff);
        var tempLowBoldness  = Boldness(prediction.PredictedTempLow,  prediction.ForecastTempLow,  MaxTempDiff);
        var precipBoldness   = Boldness(prediction.PredictedPrecipitation, prediction.ForecastPrecipitation, MaxPrecipDiff);
        var windBoldness     = Boldness(prediction.PredictedMaxWindSpeed,  prediction.ForecastMaxWindSpeed,  MaxWindDiff);

        var avgBoldness = (tempHighBoldness + tempLowBoldness + precipBoldness + windBoldness) / 4.0;

        // Time multiplier: doubles every 7 days out (exponential)
        var timeMult = Math.Pow(2.0, daysOut / 7.0);

        // Base scale of 100 points for a maximally bold prediction made 7 days out
        // timeMult at 7 days = 2, so divide by 2 to anchor 100 pts there
        prediction.PotentialPoints = (int)Math.Round(avgBoldness * 100 * timeMult / 2.0);
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

    // How bold is this prediction vs the forecast? (0 = same as forecast, 1 = max disagreement)
    // Exponential curve: small deviations earn little, large deviations earn a lot.
    private static double Boldness(double predicted, double forecast, double maxDiff)
    {
        var diff = Math.Abs(predicted - forecast);
        var normalized = Math.Min(diff / maxDiff, 1.0);
        // x^0.5 gives a concave-up curve: bold disagreements earn disproportionately more
        return Math.Pow(normalized, 0.5);
    }

    // What fraction of potential points does this field earn?
    // 1.0 = actual matched your prediction perfectly; 0.0 = actual matched the forecast perfectly.
    // Uses an exponential decay so being close to right earns most of the value.
    private static double EarningRate(double predicted, double forecast, double actual)
    {
        var totalDisagreement = Math.Abs(predicted - forecast);
        if (totalDisagreement < 1e-9) return 0.0; // prediction was same as forecast — nothing to earn

        // How far did actual end up from the original forecast, in the direction of your prediction?
        // Positive = actual moved toward your prediction; negative = moved away
        var actualDiff = actual - forecast;
        var predictedDiff = predicted - forecast;

        // Project actual movement onto prediction direction (0 = stayed at forecast, 1 = hit your prediction exactly)
        var alignment = actualDiff / predictedDiff;
        alignment = Math.Max(0.0, Math.Min(1.0, alignment));

        // Exponential reward: you have to be mostly right to earn most of the points
        // alignment^2 means 50% of the way earns only 25% of points
        return Math.Pow(alignment, 2.0);
    }
}
