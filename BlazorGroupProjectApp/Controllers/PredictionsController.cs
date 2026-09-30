using BlazorGroupProjectApp.Data;
using BlazorGroupProjectApp.Models;
using BlazorGroupProjectApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlazorGroupProjectApp.Controllers;

[Route("api/predictions")]
[ApiController]
public class PredictionsController : Controller
{
    private readonly ClimateSeerContext _db;
    private readonly PredictionService _predictionService;

    public PredictionsController(ClimateSeerContext db, PredictionService predictionService)
    {
        _db = db;
        _predictionService = predictionService;
    }

    [HttpGet]
    public async Task<ActionResult<List<Prediction>>> GetPredictions([FromQuery] int userProfileId)
    {
        // TODO: replace userProfileId param with authenticated user identity
        var predictions = await _predictionService.GetUserPredictionsAsync(userProfileId);
        return Ok(predictions);
    }

    [HttpGet("{predictionId}")]
    public async Task<ActionResult<Prediction>> GetPrediction(int predictionId, [FromQuery] int userProfileId)
    {
        var prediction = await _predictionService.GetPredictionAsync(predictionId, userProfileId);
        if (prediction == null) return NotFound();
        return Ok(prediction);
    }

    [HttpPost]
    public async Task<ActionResult<Prediction>> CreatePrediction([FromBody] Prediction prediction)
    {
        // TODO: set prediction.UserProfileId from authenticated user, not request body
        try
        {
            var created = await _predictionService.CreatePredictionAsync(prediction);
            return CreatedAtAction(nameof(GetPrediction), new { predictionId = created.Id }, created);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{predictionId}")]
    public async Task<IActionResult> DeletePrediction(int predictionId, [FromQuery] int userProfileId)
    {
        var success = await _predictionService.DeletePredictionAsync(predictionId, userProfileId);
        if (!success) return NotFound();
        return Ok();
    }
}
