using System.Text.Json.Serialization;

namespace SolarWatch.RequestsAndResponses.External;

public record SunriseSunsetExternalApiResponse
{
    [JsonPropertyName("results")]
    [JsonRequired]
    public SunriseSunsetResults Results { get; set; }
}

public record SunriseSunsetResults
{
    [JsonPropertyName("sunrise")]
    [JsonRequired]
    public string Sunrise { get; set; }

    [JsonPropertyName("sunset")]
    [JsonRequired]
    public string Sunset { get; set; }
}