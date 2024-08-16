using SolarWatch.Models;

namespace SolarWatch.Services;

public interface IGeocodeApiService
{
    public Task<IEnumerable<Coordinates>> GetCoordinatesByCityName(string city);
}