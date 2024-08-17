using SolarWatch.DTOs;

namespace SolarWatch.Services;

public interface ICityDataService
{
    public Task<List<CityWithSunriseSunsetResponse>> GetCityData(string cityName);
}