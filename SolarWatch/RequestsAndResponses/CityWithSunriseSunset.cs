using System.ComponentModel.DataAnnotations;

namespace SolarWatch.RequestsAndResponses;

public record CityWithSunriseSunset
{
    [Required] public string CityName { get; set; }

    [Required] public string Country { get; set; }

    [Required] public string? State { get; set; }

    [Required] public double Latitude { get; set; }

    [Required] public double Longitude { get; set; }

    [Required] public TimeOnly Sunrise { get; set; }

    [Required] public TimeOnly Sunset { get; set; }
}