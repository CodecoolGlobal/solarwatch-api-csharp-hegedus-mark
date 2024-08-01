using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SolarWatch.Models;

public class SunriseSunset
{
    [JsonPropertyName("sunrise")]
    [Required]
    public TimeOnly Sunrise { get; init; }

    [JsonPropertyName("sunset")]
    [Required]
    public TimeOnly Sunset { get; init; }
}