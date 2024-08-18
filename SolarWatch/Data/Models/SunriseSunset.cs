using System.ComponentModel.DataAnnotations;

namespace SolarWatch.Data.Models;

public class SunriseSunset
{
    
    [Required] public TimeOnly Sunrise { get; init; }
    [Required] public TimeOnly Sunset { get; init; }
    
}