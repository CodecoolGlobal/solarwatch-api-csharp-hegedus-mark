using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SolarWatch.DTOs;

public class SunriseSunsetApiResponseDto
{
    [JsonPropertyName("results")]
    [Required]
    public SunriseSunsetResultsDto Results { get; set; }
}

public class SunriseSunsetResultsDto
{
    [JsonPropertyName("sunrise")]
    [Required]
    public string Sunrise { get; set; }

    [JsonPropertyName("sunset")]
    [Required]
    public string Sunset { get; set; }
}