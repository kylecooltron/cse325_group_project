using BlazorGroupProjectApp.Data;
using BlazorGroupProjectApp.Models;
using BlazorGroupProjectApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BlazorGroupProjectApp.Controllers;

[Route("api/predictions")]
[ApiController]
[Authorize]
public class PredictionsController : Controller
{
    private readonly ClimateSeerContext _db;
    private readonly PredictionService _predictionService;

    public PredictionsController(ClimateSeerContext db, PredictionService predictionService)
    {
        _db = db;
        _predictionService = predictionService;
    }

    private string? CurrentUserId => User.FindFirstValue(ClaimTypes.NameIdentifier);

    [HttpGet]
    public async Task<ActionResult<List<Prediction>>> GetPredictions()
    {
        if (CurrentUserId == null) return Unauthorized();
        var predictions = await _predictionService.GetUserPredictionsAsync(CurrentUserId);
        return Ok(predictions);
    }

    [HttpGet("{predictionId}")]
    public async Task<ActionResult<Prediction>> GetPrediction(int predictionId)
    {
        if (CurrentUserId == null) return Unauthorized();
        var prediction = await _predictionService.GetPredictionAsync(predictionId, CurrentUserId);
        if (prediction == null) return NotFound();
        return Ok(prediction);
    }

    [HttpPost]
    public async Task<ActionResult<Prediction>> CreatePrediction([FromBody] Prediction prediction)
    {
        if (CurrentUserId == null) return Unauthorized();
        prediction.UserId = CurrentUserId;
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
    public async Task<IActionResult> DeletePrediction(int predictionId)
    {
        if (CurrentUserId == null) return Unauthorized();
        var success = await _predictionService.DeletePredictionAsync(predictionId, CurrentUserId);
        if (!success) return NotFound();
        return Ok();
    }
}
