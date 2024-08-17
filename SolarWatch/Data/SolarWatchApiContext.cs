using Microsoft.EntityFrameworkCore;
using SolarWatch.Models;

namespace SolarWatch.Data;

public class SolarWatchApiContext : DbContext
{
    public DbSet<City> Cities { get; set; }
    public DbSet<SunriseSunset> SunriseSunsets { get; set; }
}