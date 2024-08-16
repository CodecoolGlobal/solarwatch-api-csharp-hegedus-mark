using System.Text.Json;
using Microsoft.Extensions.Options;
using SolarWatch.Configuration;
using SolarWatch.Exceptions;
using SolarWatch.Models;

namespace SolarWatch.Services;

public class GeocodeApiService : IGeocodeApiService
{
    private const int LIMIT = 5;
    private readonly IApiService<Coordinates> _apiService;
    private readonly string _geocodeBaseUrl;
    private readonly string _geocodeApiKey;

    public GeocodeApiService(IApiService<Coordinates> apiService, IOptions<ExternalApiSettings> configuration)
    {
        _apiService = apiService;
        _geocodeBaseUrl = configuration.Value.GeocodeBaseUrl;
        _geocodeApiKey = configuration.Value.GeocodeApiKey;
    }


    public async Task<IEnumerable<Coordinates>> GetCoordinatesByCityName(string city)
    {
        string url = $"{_geocodeBaseUrl}/direct?q={city}&limit={LIMIT}&appid={_geocodeApiKey}";

        var responseString = await _apiService.GetAsync(url);

        var content = JsonSerializer.Deserialize<List<Coordinates>>(responseString);

        if (content is null || content.Count == 0)
        {
            throw new NotFoundException($"No coordinates found for city: {city}");
        }

        var matchedCities = content.Where(coordinates => coordinates.Name.Contains(city, StringComparison.CurrentCultureIgnoreCase)) ??
                            throw new NotFoundException($"City with name {city} not found!");

        return matchedCities;
    }
}