using System.Text.Json;
using Microsoft.Extensions.Options;
using SolarWatch.Configuration;
using SolarWatch.Data.Models;
using SolarWatch.DTOs;

namespace SolarWatch.Services;

public class SunriseSunsetApiService : ISunriseSunsetApiService
{
    private readonly IApiService _apiService;
    private readonly string _baseUrl;

    public SunriseSunsetApiService(IApiService apiService,
        IOptions<ExternalApiSettings> configuration)
    {
        _apiService = apiService;
        _baseUrl = configuration.Value.SunriseSunsetBaseUrl;
    }

    public async Task<SunriseSunset?> GetSunriseSunsetByCoordinates(City city)
    {
        var url = $"{_baseUrl}?lat={city.Latitude}&lng={city.Longitude}&formatted=0";

        var responseString = await _apiService.GetAsync(url);

        var content = JsonSerializer.Deserialize<SunriseSunsetExternalApiResponse>(responseString);

        if (content is null)
        {
            return null;
        }

        return MapToSunriseSunset(content);
    }

    public SunriseSunset MapToSunriseSunset(SunriseSunsetExternalApiResponse externalApiResponse)
    {
        return new SunriseSunset
        {
            Sunrise = TimeOnly.FromDateTime(DateTime.Parse(externalApiResponse.Results.Sunrise)),
            Sunset = TimeOnly.FromDateTime(DateTime.Parse(externalApiResponse.Results.Sunset)),
        };
    }
}