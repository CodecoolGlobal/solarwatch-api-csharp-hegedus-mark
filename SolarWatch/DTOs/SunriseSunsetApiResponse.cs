using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SolarWatch.DTOs;

public class SunriseSunsetApiResponse
{
    [JsonPropertyName("results")]
    [Required]
    public SunriseSunsetResults Results { get; set; }
}

public class SunriseSunsetResults
{
    [JsonPropertyName("sunrise")]
    [Required]
    public string Sunrise { get; set; }

    [JsonPropertyName("sunset")]
    [Required]
    public string Sunset { get; set; }
}