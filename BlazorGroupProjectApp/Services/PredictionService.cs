using BlazorGroupProjectApp.Data;
using BlazorGroupProjectApp.Models;
using Microsoft.EntityFrameworkCore;

namespace BlazorGroupProjectApp.Services;

public class PredictionService
{
    private readonly ClimateSeerContext _db;
    private readonly WeatherService _weather;
    private readonly ScoringService _scoring;

    public PredictionService(ClimateSeerContext db, WeatherService weather, ScoringService scoring)
    {
        _db = db;
        _weather = weather;
        _scoring = scoring;
    }

    public async Task<List<Prediction>> GetUserPredictionsAsync(string userId)
    {
        return await _db.Predictions
            .Where(p => p.UserId == userId)
            .Include(p => p.Result)
            .OrderBy(p => p.TargetDate)
            .ThenBy(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task<Prediction?> GetPredictionAsync(int predictionId, string userId)
    {
        return await _db.Predictions
            .Include(p => p.Result)
            .SingleOrDefaultAsync(p => p.Id == predictionId && p.UserId == userId);
    }

    public async Task<Prediction> CreatePredictionAsync(Prediction prediction)
    {
        if (prediction.TargetDate < DateTime.UtcNow.AddHours(48).Date)
            throw new InvalidOperationException("Target date must be at least 48 hours in the future.");

        if (prediction.TargetDate > DateTime.UtcNow.AddDays(14).Date)
            throw new InvalidOperationException("Target date cannot be more than 14 days in the future.");

        if (DateTime.UtcNow > prediction.Deadline)
            throw new InvalidOperationException("Prediction deadline has passed.");

        // Re-fetch the forecast server-side so the client can't spoof it
        var forecast = await _weather.GetForecastAsync(prediction.Location, prediction.TargetDate);
        if (forecast is null)
            throw new InvalidOperationException("Could not retrieve forecast for that location and date.");

        prediction.ForecastTempHigh = forecast.TempHigh;
        prediction.ForecastTempLow = forecast.TempLow;
        prediction.ForecastPrecipitation = forecast.Precipitation;
        prediction.ForecastMaxWindSpeed = forecast.MaxWindSpeed;

        if (!_scoring.IsWorthSubmitting(prediction))
            throw new InvalidOperationException("Your prediction is too similar to the current forecast. Change at least one field to make a real bet.");

        _scoring.SetPotentialPoints(prediction);

        _db.Predictions.Add(prediction);
        await _db.SaveChangesAsync();
        return prediction;
    }

    public async Task<bool> DeletePredictionAsync(int predictionId, string userId)
    {
        var prediction = await _db.Predictions
            .SingleOrDefaultAsync(p => p.Id == predictionId && p.UserId == userId);

        if (prediction == null) return false;
        if (DateTime.UtcNow > prediction.Deadline) return false;

        _db.Predictions.Remove(prediction);
        await _db.SaveChangesAsync();
        return true;
    }
}
