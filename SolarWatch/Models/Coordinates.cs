using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SolarWatch.Models;

public class Coordinates
{
    [JsonPropertyName("lat")] [Required] public double Latitude { get; set; }

    [JsonPropertyName("lon")] [Required] public double Longitude { get; set; }
}