using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using SolarWatch.Configuration;
using SolarWatch.DTOs;
using SolarWatch.Models;
using SolarWatch.Services;

namespace SolarWatch.Test.IntegrationTests;

public class SunriseSunsetApiServiceTests
{
    private SunriseSunsetApiService _sunriseSunsetApiService;

    [SetUp]
    public void Setup()
    {
        // Set up the configuration to use user-secrets
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.Test.json", optional: false, reloadOnChange: true)
            .Build();

        var settings = new ExternalApiSettings
        {
            SunriseSunsetBaseUrl = configuration["SunriseSunsetBaseUrl"], 
        };

        var apiConfig = new ApiServiceConfiguration { MaxRetries = 1, RetryDelayMilliseconds = 0 };

        var options = Options.Create(settings);
        var apiService = new ApiService<SunriseSunsetApiResponseDto>(new HttpClient(), apiConfig);

        _sunriseSunsetApiService = new SunriseSunsetApiService(apiService, options);
    }

    [Test]
    public async Task GetSunriseSunsetByCoordinates_ReturnsSunriseAndSunset()
    {
        var coordinates = new Coordinates { Longitude = 0, Latitude = 0 };

        var result = await _sunriseSunsetApiService.GetSunriseSunsetByCoordinates(coordinates);
        
        Assert.IsNotNull(result);
        Assert.That(result.Sunrise, Is.Not.EqualTo(null));
        Assert.That(result.Sunset, Is.Not.EqualTo(null));
    }
}