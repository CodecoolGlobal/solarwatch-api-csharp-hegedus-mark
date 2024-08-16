using System.Text.Json;
using Microsoft.Extensions.Options;
using SolarWatch.Configuration;
using SolarWatch.DTOs;
using SolarWatch.Exceptions;
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

    public async Task<SunriseSunset?> GetSunriseSunsetByCoordinates(Coordinates coordinates)
    {
        var url = $"{_baseUrl}?lat={coordinates.Latitude}&lng={coordinates.Longitude}&formatted=0";

        var responseString = await _apiService.GetAsync(url);

        var content = JsonSerializer.Deserialize<SunriseSunsetApiResponse>(responseString);

        if (content is null)
        {
            return null;
        }

        return MapToSunriseSunset(content, coordinates);
    }

    public SunriseSunset MapToSunriseSunset(SunriseSunsetApiResponse apiResponse, Coordinates coordinates)
    {
        if (apiResponse?.Results == null)
        {
            throw new ArgumentNullException(nameof(apiResponse));
        }

        return new SunriseSunset
        {
            Sunrise = TimeOnly.FromDateTime(DateTime.Parse(apiResponse.Results.Sunrise)),
            Sunset = TimeOnly.FromDateTime(DateTime.Parse(apiResponse.Results.Sunset)),
            Name = coordinates.Name,
            Country = coordinates.Country,
            State = coordinates.State
        };
    }
}