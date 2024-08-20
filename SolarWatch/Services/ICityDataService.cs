using SolarWatch.RequestsAndResponses;

namespace SolarWatch.Services;

public interface ICityDataService
{
    public Task<List<CityWithSunriseSunset>> GetCityData(string cityName);
}