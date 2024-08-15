using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SolarWatch.Models;

public struct Coordinates
{
    [JsonPropertyName("lat")]
    [JsonRequired]
    public double Latitude { get; set; }

    [JsonPropertyName("lon")]
    [JsonRequired]
    public double Longitude { get; set; }

    [JsonPropertyName("name")]
    [JsonRequired]
    public string Name { get; set; }
}