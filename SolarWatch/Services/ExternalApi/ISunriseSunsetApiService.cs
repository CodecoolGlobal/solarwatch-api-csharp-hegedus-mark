using SolarWatch.Data.Models;

namespace SolarWatch.Services;

public interface ISunriseSunsetApiService
{
    public Task<SunriseSunset?> GetSunriseSunsetByCoordinates(City city);
}