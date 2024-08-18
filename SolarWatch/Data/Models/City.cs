using System.ComponentModel.DataAnnotations;

namespace SolarWatch.Data.Models;

public class City
{
    private const double TOLERANCE = 0.01;

    public int CityId { get; set; }

    [Required] public double Latitude { get; set; }

    [Required] public double Longitude { get; set; }

    [Required] public string Name { get; set; }

    [Required] public string Country { get; set; }

    public string? State { get; set; }

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