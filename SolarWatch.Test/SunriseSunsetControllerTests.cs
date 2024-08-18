using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using NSubstitute.ExceptionExtensions;
using SolarWatch.Controllers;
using SolarWatch.Exceptions;
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
        /*_controller = new SunriseSunsetController(_geocodeApiService, _sunriseSunsetApiService);*/
    }
}