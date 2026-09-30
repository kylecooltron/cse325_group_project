using BlazorGroupProjectApp.Data;
using BlazorGroupProjectApp.Models;
using Microsoft.EntityFrameworkCore;

namespace BlazorGroupProjectApp.Services;

public class ScoringService
{
    private readonly ClimateSeerContext _db;

    public ScoringService(ClimateSeerContext db)
    {
        _db = db;
    }

    public PredictionResult Score(Prediction prediction, WeatherObservation actual)
    {
        var tempAccuracy = CalculateAccuracy(prediction.PredictedTemperature, actual.Temperature, tolerance: 5);
        var precipAccuracy = CalculateAccuracy(prediction.PredictedPrecipitation, actual.Precipitation, tolerance: 0.25);
        var windAccuracy = CalculateAccuracy(prediction.PredictedWindSpeed, actual.WindSpeed, tolerance: 5);

        var points = (int)((tempAccuracy + precipAccuracy + windAccuracy) / 3 * 100);

        return new PredictionResult
        {
            PredictionId = prediction.Id,
            ActualTemperature = actual.Temperature,
            ActualPrecipitation = actual.Precipitation,
            ActualWindSpeed = actual.WindSpeed,
            TemperatureAccuracy = tempAccuracy,
            PrecipitationAccuracy = precipAccuracy,
            WindAccuracy = windAccuracy,
            Points = points,
            EvaluatedAt = DateTime.UtcNow
        };
    }

    public async Task EvaluatePendingPredictionsAsync()
    {
        // TODO: query predictions past their target date without results and score them
        await Task.CompletedTask;
    }

    private static double CalculateAccuracy(double predicted, double actual, double tolerance)
    {
        var diff = Math.Abs(predicted - actual);
        if (diff >= tolerance) return 0;
        return 1.0 - (diff / tolerance);
    }
}
