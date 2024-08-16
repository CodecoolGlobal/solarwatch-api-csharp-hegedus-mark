using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SolarWatch.Models;

public class Coordinates
{
    private const double TOLERANCE = 0.01;

    [JsonPropertyName("lat")]
    [JsonRequired]
    [Required]
    public double Latitude { get; init; }

    [JsonPropertyName("lon")]
    [JsonRequired]
    [Required]
    public double Longitude { get; init; }

    [JsonPropertyName("name")]
    [JsonRequired]
    [Required]
    public string Name { get; init; }

    [JsonPropertyName("country")]
    [JsonRequired]
    [Required]
    public string Country { get; init; }

    [JsonPropertyName("state")]
    [Required]
    [JsonRequired]
    public string State { get; init; }


    public override bool Equals(object obj)
    {
        if (obj is Coordinates other)
        {
            return Math.Abs(Latitude - other.Latitude) < TOLERANCE &&
                   Math.Abs(Longitude - other.Longitude) < TOLERANCE &&
                   Name == other.Name &&
                   State == other.State &&
                   Country == other.Country;
        }

        return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Latitude, Longitude, Name, State, Country);
    }
}