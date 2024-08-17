using System.Text.Json;
using Microsoft.Extensions.Options;
using SolarWatch.Configuration;
using SolarWatch.DTOs;
using SolarWatch.Models;
using SolarWatch.Services;

namespace SolarWatch.Test;

public class SunriseSunsetApiServiceTests
{
    private SunriseSunsetApiService _sunriseSunsetApiService;
    private IApiService _mockApiService;
    private IOptions<ExternalApiSettings> _mockOptions;

    [SetUp]
    public void Setup()
    {
        // Mock the IApiService<SunriseSunset>
        _mockApiService = Substitute.For<IApiService>();

        // Mock the configuration
        var mockSettings = new ExternalApiSettings
        {
            SunriseSunsetBaseUrl = "https://api.sunrise-sunset.org"
        };
        _mockOptions = Substitute.For<IOptions<ExternalApiSettings>>();
        _mockOptions.Value.Returns(mockSettings);

        // Create the service instance with mocked dependencies
        _sunriseSunsetApiService = new SunriseSunsetApiService(_mockApiService, _mockOptions);
    }

    [Test]
    public async Task GetSunriseSunsetByCoordinates_ShouldCallApiServiceWithCorrectUrl()
    {
        // Arrange
        var url = "https://api.sunrise-sunset.org?lat=0&lng=0&formatted=0";
        var sunriseString = "2024-07-31T05:21:48+00:00";
        var sunsetString = "2024-07-31T19:26:19+00:00";
        var coordinates = new City { Latitude = 0, Longitude = 0 };
        var apiResponseDto = new SunriseSunsetExternalApiResponse
        {
            Results = new SunriseSunsetResults { Sunrise = sunriseString, Sunset = sunsetString }
        };
        var expected = new SunriseSunset
        {
            Sunrise = TimeOnly.FromDateTime(DateTime.Parse(sunriseString)),
            Sunset = TimeOnly.FromDateTime(DateTime.Parse(sunsetString))
        };

        _mockApiService.GetAsync(url).Returns(JsonSerializer.Serialize(apiResponseDto));

        // Act
        var result = await _sunriseSunsetApiService.GetSunriseSunsetByCoordinates(coordinates);

        // Assert
        Assert.That(result.Sunset, Is.EqualTo(expected.Sunset));
        Assert.That(result.Sunrise, Is.EqualTo(expected.Sunrise));
        await _mockApiService.Received(1).GetAsync(url);
    }

    [Test]
    public void MapToSunriseSunset_ShouldReturnCorrectSunriseSunset()
    {
        // Arrange
        var sunriseString = "2024-07-31T05:21:48+00:00";
        var sunsetString = "2024-07-31T19:26:19+00:00";
        var apiResponseDto = new SunriseSunsetExternalApiResponse
        {
            Results = new SunriseSunsetResults { Sunrise = sunriseString, Sunset = sunsetString }
        };
        var coordinates = new City { Name = "test", State = "test", Country = "test" };
        var expected = new SunriseSunset
        {
            Sunrise = TimeOnly.FromDateTime(DateTime.Parse(sunriseString)),
            Sunset = TimeOnly.FromDateTime(DateTime.Parse(sunsetString))
        };

        // Act
        var result = _sunriseSunsetApiService.MapToSunriseSunset(apiResponseDto);

        // Assert
        Assert.That(result.Sunrise, Is.EqualTo(expected.Sunrise));
        Assert.That(result.Sunset, Is.EqualTo(expected.Sunset));
    }

    [Test]
    public void MapToSunriseSunset_ShouldThrowArgumentNullException_WhenApiResponseDtoIsNull()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _sunriseSunsetApiService.MapToSunriseSunset(null));
    }

    [Test]
    public void MapToSunriseSunset_ShouldThrowArgumentNullException_WhenResultsIsNull()
    {
        // Arrange
        var apiResponseDto = new SunriseSunsetExternalApiResponse
        {
            Results = null
        };


        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            _sunriseSunsetApiService.MapToSunriseSunset(apiResponseDto));
    }
}