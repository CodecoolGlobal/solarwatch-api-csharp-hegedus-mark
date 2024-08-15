using System.Text.Json;
using Microsoft.Extensions.Options;
using SolarWatch.Configuration;
using SolarWatch.Models;

namespace SolarWatch.Services;

public class GeocodeApiService : IGeocodeApiService
{
    private readonly IApiService<Coordinates> _apiService;
    private readonly string _geocodeBaseUrl;
    private readonly string _geocodeApiKey;

    public GeocodeApiService(IApiService<Coordinates> apiService, IOptions<ExternalApiSettings> configuration)
    {
        _apiService = apiService;
        _geocodeBaseUrl = configuration.Value.GeocodeBaseUrl;
        _geocodeApiKey = configuration.Value.GeocodeApiKey;
    }


    public async Task<Coordinates> GetCoordinatesByCityName(string city)
    {
        string url = $"{_geocodeBaseUrl}/direct?q={city}&limit=1&appid={_geocodeApiKey}";

        var responseString = await _apiService.GetAsync(url);

        var content = JsonSerializer.Deserialize<List<Coordinates>>(responseString);

        return content[0];
    }
}