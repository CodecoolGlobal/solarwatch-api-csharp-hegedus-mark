using Microsoft.Extensions.Options;
using SolarWatch.Configuration;
using SolarWatch.DTOs;
using SolarWatch.Models;

namespace SolarWatch.Services;

public class SunriseSunsetApiService : ISunriseSunsetApiService
{
    private readonly IApiService<SunriseSunsetApiResponseDto> _apiService;
    private readonly string _baseUrl;

    public SunriseSunsetApiService(IApiService<SunriseSunsetApiResponseDto> apiService,
        IOptions<ExternalApiSettings> configuration)
    {
        _apiService = apiService;
        _baseUrl = configuration.Value.SunriseSunsetBaseUrl;
    }

    public async Task<SunriseSunset> GetSunriseSunsetByCoordinates(Coordinates coordinates)
    {
        var url = $"{_baseUrl}?lat={coordinates.Latitude}&lng={coordinates.Longitude}&formatted=0";

        var response = await _apiService.GetAsync(url);

        return MapToSunriseSunset(response);
    }

    public SunriseSunset MapToSunriseSunset(SunriseSunsetApiResponseDto apiResponseDto)
    {
        if (apiResponseDto?.Results == null)
        {
            throw new ArgumentNullException(nameof(apiResponseDto));
        }

        return new SunriseSunset
        {
            Sunrise = TimeOnly.FromDateTime(DateTime.Parse(apiResponseDto.Results.Sunrise)),
            Sunset = TimeOnly.FromDateTime(DateTime.Parse(apiResponseDto.Results.Sunset))
        };
    }
}