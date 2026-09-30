using BlazorGroupProjectApp.Data;
using BlazorGroupProjectApp.Models;
using Microsoft.EntityFrameworkCore;

namespace BlazorGroupProjectApp.Services;

public class PredictionService
{
    private readonly ClimateSeerContext _db;

    public PredictionService(ClimateSeerContext db)
    {
        _db = db;
    }

    public async Task<List<Prediction>> GetUserPredictionsAsync(int userProfileId)
    {
        return await _db.Predictions
            .Where(p => p.UserProfileId == userProfileId)
            .Include(p => p.Result)
            .OrderByDescending(p => p.TargetDate)
            .ToListAsync();
    }

    public async Task<Prediction?> GetPredictionAsync(int predictionId, int userProfileId)
    {
        return await _db.Predictions
            .Include(p => p.Result)
            .SingleOrDefaultAsync(p => p.Id == predictionId && p.UserProfileId == userProfileId);
    }

    public async Task<Prediction> CreatePredictionAsync(Prediction prediction)
    {
        if (prediction.TargetDate <= DateTime.UtcNow)
            throw new InvalidOperationException("Target date must be in the future.");

        if (DateTime.UtcNow > prediction.Deadline)
            throw new InvalidOperationException("Prediction deadline has passed.");

        _db.Predictions.Add(prediction);
        await _db.SaveChangesAsync();
        return prediction;
    }

    public async Task<bool> DeletePredictionAsync(int predictionId, int userProfileId)
    {
        var prediction = await _db.Predictions
            .SingleOrDefaultAsync(p => p.Id == predictionId && p.UserProfileId == userProfileId);

        if (prediction == null) return false;
        if (DateTime.UtcNow > prediction.Deadline) return false;

        _db.Predictions.Remove(prediction);
        await _db.SaveChangesAsync();
        return true;
    }
}
