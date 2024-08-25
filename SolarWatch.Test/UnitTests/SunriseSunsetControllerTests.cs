using SolarWatch.Controllers;
using SolarWatch.Services;

namespace SolarWatch.Test.UnitTests;

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
        /*_controller = new SunriseSunsetController(_geocodeApiService, _sunriseSunsetApiService);*/
    }
}