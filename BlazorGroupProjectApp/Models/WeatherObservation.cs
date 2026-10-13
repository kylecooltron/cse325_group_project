namespace BlazorGroupProjectApp.Models;

public class WeatherObservation
{
    public int Id { get; set; }
    public string Location { get; set; } = string.Empty;
    public DateTime ObservedAt { get; set; }
    public double TempHigh { get; set; }
    public double TempLow { get; set; }
    public double Precipitation { get; set; }
    public double MaxWindSpeed { get; set; }
    public string Description { get; set; } = string.Empty;
}
