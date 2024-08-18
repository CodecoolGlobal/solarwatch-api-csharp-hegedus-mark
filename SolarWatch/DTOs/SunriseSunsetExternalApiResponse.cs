using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SolarWatch.DTOs;

public class SunriseSunsetExternalApiResponse
{
    [JsonPropertyName("results")]
    [JsonRequired]
    public SunriseSunsetResults Results { get; set; }
}

public class SunriseSunsetResults
{
    [JsonPropertyName("sunrise")]
    [JsonRequired]
    public string Sunrise { get; set; }

    [JsonPropertyName("sunset")]
    [JsonRequired]
    public string Sunset { get; set; }
}