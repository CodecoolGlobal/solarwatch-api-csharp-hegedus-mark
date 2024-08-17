using System.Text.Json;
using Microsoft.Extensions.Options;
using SolarWatch.Configuration;
using SolarWatch.DTOs;
using SolarWatch.Exceptions;
using SolarWatch.Models;

namespace SolarWatch.Services;

public class GeocodeApiService : IGeocodeApiService
{
    public const int LIMIT = 5;
    private readonly IApiService _apiService;
    private readonly string _geocodeBaseUrl;
    private readonly string _geocodeApiKey;

    public GeocodeApiService(IApiService apiService, IOptions<ExternalApiSettings> configuration)
    {
        _apiService = apiService;
        _geocodeBaseUrl = configuration.Value.GeocodeBaseUrl;
        _geocodeApiKey = configuration.Value.GeocodeApiKey;
    }


    public async Task<IEnumerable<City>> GetCoordinatesByCityName(string city)
    {
        string url = $"{_geocodeBaseUrl}/direct?q={city}&limit={LIMIT}&appid={_geocodeApiKey}";

        var responseString = await _apiService.GetAsync(url);

        var content = JsonSerializer.Deserialize<List<CoordinatesExternalApiResponse>>(responseString);

        if (content is null || content.Count == 0)
        {
            throw new NotFoundException($"No coordinates found for city: {city}");
        }

        var matchedCities = content.Where(coordinates =>
            coordinates.Name.Contains(city, StringComparison.CurrentCultureIgnoreCase)).ToList();

        if (matchedCities.Count == 0)
        {
            throw new NotFoundException($"No matched Cities found for city: {city}");
        }

        var mappedCities = matchedCities.Select(MapToCity).ToList();

        return mappedCities;
    }


    private static City MapToCity(CoordinatesExternalApiResponse coordinatesExternal)
    {
        return new City
        {
            Name = coordinatesExternal.Name,
            Country = coordinatesExternal.Country,
            Latitude = coordinatesExternal.Latitude,
            Longitude = coordinatesExternal.Longitude,
            State = coordinatesExternal.State,
        };
    }
}