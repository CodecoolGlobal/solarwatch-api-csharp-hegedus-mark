using SolarWatch.RequestsAndResponses;

namespace SolarWatch.Services;

public interface ICityDataService
{
    public Task<List<CityWithSunriseSunsetResponse>> GetCityData(string cityName);
}