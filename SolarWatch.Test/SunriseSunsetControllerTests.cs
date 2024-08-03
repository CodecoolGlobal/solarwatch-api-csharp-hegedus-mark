using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using NSubstitute.ExceptionExtensions;
using SolarWatch.Controllers;
using SolarWatch.Exceptions;
using SolarWatch.Models;
using SolarWatch.Services;

namespace SolarWatch.Test;

public class SunriseSunsetControllerTests
{
    private SunriseSunsetController _controller;
    private IGeocodeApiService _geocodeApiService;
    private ISunriseSunsetApiService _sunriseSunsetApiService;

    [SetUp]
    public void Setup()
    {
        _geocodeApiService = Substitute.For<IGeocodeApiService>();
        _sunriseSunsetApiService = Substitute.For<ISunriseSunsetApiService>();
        _controller = new SunriseSunsetController(_geocodeApiService, _sunriseSunsetApiService);
    }

    [Test]
    public async Task GetSunriseSunsetByCity_ReturnsOkResult_WhenDataIsValid()
    {
        // Arrange
        var city = "New York";
        var coordinates = new Coordinates { Latitude = 40.7128, Longitude = -74.0060 };
        var sunriseSunset = new SunriseSunset { Sunrise = new TimeOnly(6, 0), Sunset = new TimeOnly(18, 0) };

        _geocodeApiService.GetCoordinatesByCityName(city).Returns(Task.FromResult(coordinates));
        _sunriseSunsetApiService.GetSunriseSunsetByCoordinates(coordinates).Returns(Task.FromResult(sunriseSunset));

        // Act
        var actionResult = await _controller.GetSunriseSunsetByCity(city);
        var result = actionResult.Result as OkObjectResult;

        // Assert
        Assert.IsNotNull(result);
        Assert.That(result.StatusCode, Is.EqualTo(200));
        Assert.That(result.Value, Is.EqualTo(sunriseSunset));
    }
}