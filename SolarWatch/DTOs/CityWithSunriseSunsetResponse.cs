namespace SolarWatch.DTOs;

public class CityWithSunriseSunsetResponse
{
    public string Name { get; set; }
    public string Country { get; set; }
    public string State { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public TimeOnly Sunrise { get; set; }
    public TimeOnly Sunset { get; set; }
}