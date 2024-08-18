using System.Text.Json.Serialization;

namespace SolarWatch.RequestsAndResponses.External;

public record CoordinatesExternalApiResponse
{
    [JsonPropertyName("lat")]
    [JsonRequired]
    public double Latitude { get; init; }

    [JsonPropertyName("lon")]
    [JsonRequired]
    public double Longitude { get; init; }

    [JsonPropertyName("name")]
    [JsonRequired]
    public string Name { get; init; }

    [JsonPropertyName("country")]
    [JsonRequired]
    public string Country { get; init; }

    [JsonPropertyName("state")]
    public string? State { get; init; }
}