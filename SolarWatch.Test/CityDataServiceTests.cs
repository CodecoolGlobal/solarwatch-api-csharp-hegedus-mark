using Microsoft.Extensions.Logging;
using SolarWatch.Data.Models;
using SolarWatch.Data.Repositories;
using SolarWatch.Services;

namespace SolarWatch.Test;

public class CityDataServiceTests
{
    private ICityRepository _cityRepository;
    private IGeocodeApiService _geocodeApiService;
    private ISunriseSunsetApiService _sunriseSunsetApiService;
    private CityDataService _cityDataService;


    [SetUp]
    public void Setup()
    {
        _cityRepository = Substitute.For<ICityRepository>();
        _geocodeApiService = Substitute.For<IGeocodeApiService>();
        _sunriseSunsetApiService = Substitute.For<ISunriseSunsetApiService>();
        var logger = Substitute.For<ILogger<CityDataService>>();

        _cityDataService = new CityDataService(_cityRepository, _geocodeApiService, _sunriseSunsetApiService, logger);
    }

    [Test]
    public async Task GetCityData_FoundInRepository_ShouldReturnCityDataFromRepository()
    {
        //Arrange
        string cityName = "FoundCityName";
        List<City> cities =
        [
            new City
            {
                Name = cityName, Longitude = 10, Latitude = 20, Country = "Test", SunriseSunset = GetTestSunriseSunset()
            }
        ];
        _cityRepository.GetByName(cityName).Returns(cities);

        //Act
        var result = await _cityDataService.GetCityData(cityName);

        //Assert
        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result[0].Name, Is.EqualTo(cityName));
    }

    [Test]
    public async Task GetCityData_NotFoundInRepository_ShouldFetchFromApiAndReturnData()
    {
        // Arrange
        string cityName = "NotFoundCityName";
        _cityRepository.GetByName(cityName).Returns(new List<City>());

        var apiCities = new List<City>
        {
            new City
            {
                Name = cityName,
                Longitude = 30,
                Latitude = 40,
                Country = "API_Test",
                SunriseSunset = GetTestSunriseSunset()
            }
        };

        _geocodeApiService.GetCoordinatesByCityName(cityName).Returns(apiCities);

        // Simulate fetching sunrise/sunset data for each city
        _sunriseSunsetApiService.GetSunriseSunsetByCoordinates(Arg.Any<City>())
            .Returns(apiCities[0].SunriseSunset);

        // Act
        var result = await _cityDataService.GetCityData(cityName);

        // Assert
        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result[0].Name, Is.EqualTo(cityName));
        Assert.That(result[0].Country, Is.EqualTo("API_Test"));
        _cityRepository.Received(1).Add(Arg.Any<City>());
    }

    [Test]
    public async Task GetCityData_ApiReturnsNoData_ShouldReturnEmptyList()
    {
        // Arrange
        string cityName = "NoDataCity";
        _cityRepository.GetByName(cityName).Returns(new List<City>());

        _geocodeApiService.GetCoordinatesByCityName(cityName).Returns(new List<City>());

        // Act
        var result = await _cityDataService.GetCityData(cityName);

        // Assert
        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task GetCityData_ApiReturnsCityWithoutSunriseSunset_ShouldNotAddToRepository()
    {
        // Arrange
        string cityName = "CityWithoutSunriseSunset";
        _cityRepository.GetByName(cityName).Returns(new List<City>());

        var apiCities = new List<City>
        {
            new City
            {
                Name = cityName,
                Longitude = 50,
                Latitude = 60,
                Country = "API_Test"
            }
        };

        _geocodeApiService.GetCoordinatesByCityName(cityName).Returns(apiCities);

        _sunriseSunsetApiService.GetSunriseSunsetByCoordinates(Arg.Any<City>())
            .Returns((SunriseSunset?)null);

        // Act
        var result = await _cityDataService.GetCityData(cityName);

        // Assert
        Assert.That(result, Is.Empty);
        _cityRepository.DidNotReceive().Add(Arg.Any<City>());
    }


    private SunriseSunset GetTestSunriseSunset()
    {
        return new SunriseSunset()
        {
            Sunrise = TimeOnly.FromDateTime(new DateTime()),
            Sunset = TimeOnly.FromDateTime(new DateTime())
        };
    }
}