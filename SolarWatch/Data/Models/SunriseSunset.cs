using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SolarWatch.Models;

public class SunriseSunset
{
    
    public int SunriseSunsetId { get; set; }
    
    [Required]
    public TimeOnly Sunrise { get; init; }
    
    [Required]
    public TimeOnly Sunset { get; init; }
    
}