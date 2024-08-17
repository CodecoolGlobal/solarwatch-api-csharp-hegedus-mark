using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SolarWatch.Models;

public class City
{
    private const double TOLERANCE = 0.01;
    
    public int CityId { get; set; }
    
    [Required]
    public double Latitude { get; init; }
    
    [Required]
    public double Longitude { get; init; }
    
    [Required]
    public string Name { get; init; }
    
    [Required]
    public string Country { get; init; }
    
    [Required]
    public string State { get; init; }
    
    
    public SunriseSunset SunriseSunset { get; set; }

    public override bool Equals(object obj)
    {
        if (obj is City other)
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