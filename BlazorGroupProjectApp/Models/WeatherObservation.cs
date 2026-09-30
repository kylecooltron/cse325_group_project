namespace BlazorGroupProjectApp.Models;

public class WeatherObservation
{
    public int Id { get; set; }
    public string Location { get; set; } = string.Empty;
    public DateTime ObservedAt { get; set; }
    public double Temperature { get; set; }
    public double Precipitation { get; set; }
    public double WindSpeed { get; set; }
    public string Description { get; set; } = string.Empty;
}
