using Microsoft.EntityFrameworkCore;
using SolarWatch.Data.Models;

namespace SolarWatch.Data;

public class SolarWatchDbContext : DbContext
{
    public DbSet<City> Cities { get; set; }

    public SolarWatchDbContext(DbContextOptions<SolarWatchDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<City>()
            .OwnsOne(c => c.SunriseSunset, sb =>
            {
                sb.Property(ss => ss.Sunrise)
                    .HasColumnName(@"Sunrise")
                    .HasColumnType("TIME");
                sb.Property(ss => ss.Sunset)
                    .HasColumnName(@"Sunset")
                    .HasColumnType("TIME");
            });
    }
}