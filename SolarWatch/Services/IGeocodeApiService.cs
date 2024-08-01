using SolarWatch.Models;

namespace SolarWatch.Services;

public interface IGeocodeApiService
{
    public Task<Coordinates> GetCoordinatesByCityName(string city);
}