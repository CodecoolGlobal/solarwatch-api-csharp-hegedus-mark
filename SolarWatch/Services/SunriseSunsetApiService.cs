using System.Text.Json;
using Microsoft.Extensions.Options;
using SolarWatch.Configuration;
using SolarWatch.DTOs;
using SolarWatch.Models;

namespace SolarWatch.Services;

public class SunriseSunsetApiService : ISunriseSunsetApiService
{
    private readonly IApiService<SunriseSunsetApiResponse> _apiService;
    private readonly string _baseUrl;

    public SunriseSunsetApiService(IApiService<SunriseSunsetApiResponse> apiService,
        IOptions<ExternalApiSettings> configuration)
    {
        _apiService = apiService;
        _baseUrl = configuration.Value.SunriseSunsetBaseUrl;
    }

    public async Task<SunriseSunset> GetSunriseSunsetByCoordinates(Coordinates coordinates)
    {
        var url = $"{_baseUrl}?lat={coordinates.Latitude}&lng={coordinates.Longitude}&formatted=0";

        var responseString = await _apiService.GetAsync(url);

        var content = JsonSerializer.Deserialize<SunriseSunsetApiResponse>(responseString);

        return MapToSunriseSunset(content);
    }

    public SunriseSunset MapToSunriseSunset(SunriseSunsetApiResponse apiResponse)
    {
        if (apiResponse?.Results == null)
        {
            throw new ArgumentNullException(nameof(apiResponse));
        }

        return new SunriseSunset
        {
            Sunrise = TimeOnly.FromDateTime(DateTime.Parse(apiResponse.Results.Sunrise)),
            Sunset = TimeOnly.FromDateTime(DateTime.Parse(apiResponse.Results.Sunset))
        };
    }
}