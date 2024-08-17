using SolarWatch.Models;

namespace SolarWatch.Services;

public interface IGeocodeApiService
{
    public Task<IEnumerable<City>> GetCoordinatesByCityName(string city);
}