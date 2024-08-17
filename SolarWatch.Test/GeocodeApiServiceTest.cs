using Microsoft.Extensions.Options;
using SolarWatch.Configuration;
using SolarWatch.DTOs;
using SolarWatch.Exceptions;
using SolarWatch.Models;
using SolarWatch.Services;
using JsonException = System.Text.Json.JsonException;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace SolarWatch.Test;

public class GeocodeApiServiceTest
{
    private const string BASE_URL = "https://api.geocode.com";
    private const string API_KEY = "test_api_key";
    private GeocodeApiService _geocodeApiService;
    private IApiService _mockApiService;
    private IOptions<ExternalApiSettings> _mockOptions;
    private const int LIMIT = GeocodeApiService.LIMIT;

    [SetUp]
    public void Setup()
    {
        // Mock the IApiService<Coordinates>
        _mockApiService = Substitute.For<IApiService>();


        // Mock the configuration
        var mockSettings = new ExternalApiSettings
        {
            GeocodeBaseUrl = BASE_URL,
            GeocodeApiKey = API_KEY
        };
        _mockOptions = Substitute.For<IOptions<ExternalApiSettings>>();
        _mockOptions.Value.Returns(mockSettings);

        // Create the service instance with mocked dependencies
        _geocodeApiService = new GeocodeApiService(_mockApiService, _mockOptions);
    }

    [Test]
    public async Task GetCoordinatesByCityName_OnlyOneResult_ShouldReturnFirstItem()
    {
        // Arrange
        var cityName = "London";
        var expectedCoordinates = new List<City>
        {
            new() { Latitude = 51.5074, Longitude = -0.1278, Name = cityName }
        };
        var url =
            $"{BASE_URL}/direct?q={cityName}&limit={LIMIT}&appid={API_KEY}";

        _mockApiService.GetAsync(url).Returns(JsonSerializer.Serialize(expectedCoordinates));

        // Act
        var results = await _geocodeApiService.GetCoordinatesByCityName(cityName);
        var result = results.ToList()[0];

        // Assert
        Assert.That(result.Name, Is.EqualTo(cityName));
        await _mockApiService.Received(1).GetAsync(url);
    }

    [Test]
    public void GetCoordinatesByCityName_NonExistentCity_ShouldThrowNotFoundException()
    {
        var cityName = "Non-Existent-City";
        var url = $"{BASE_URL}/direct?q={cityName}&limit={LIMIT}&appid={API_KEY}";
        _mockApiService.GetAsync(url).Returns("[]");

        // Act
        // Assert
        Assert.ThrowsAsync<NotFoundException>(() => _geocodeApiService.GetCoordinatesByCityName(cityName));
    }

    [TestCase("[{}]")] //empty object in an array
    [TestCase("[\"test\":\"test\"]")] //not coordinates
    [TestCase("notJsonFormat")] //not json
    [TestCase(
        "[ \"lat\": \"0\", \"long\": \"0\", \"name\":\"testCity\", \"badSecondItem\":\"test\"]")] //second item in wrong format
    [TestCase("[\"lat\": \"0\", \"long\": \"0\"]")] //missingCityName
    public void GetCoordinatesByCityName_WrongData_ThrowsJsonException(string response)
    {
        var cityName = "testCity";
        var url = $"{BASE_URL}/direct?q={cityName}&limit={LIMIT}&appid={API_KEY}";
        _mockApiService.GetAsync(url).Returns(response);

        Assert.ThrowsAsync<JsonException>(() => _geocodeApiService.GetCoordinatesByCityName(cityName));
    }

    [Test]
    public async Task GetCoordinatesByCityName_MultipleItems_ReturnsCitiesThatMatchesTheRequest()
    {
        // Create a list of Coordinates objects
        var cities = new List<City>
        {
            new City
                { Latitude = 40.7128, Longitude = -74.0060, Name = "testCity", State = "test", Country = "test" },
            new City
                { Latitude = 34.0522, Longitude = -118.2437, Name = "testCityAlpha", State = "test", Country = "test" },
            new City
                { Latitude = 37.7749, Longitude = -122.4194, Name = "test", State = "test", Country = "test" }
        };

        // Serialize the list to a JSON string
        var jsonResponse = JsonSerializer.Serialize(cities);

        var requestedCityName = "testCity";
        var url =
            $"{BASE_URL}/direct?q={requestedCityName}&limit={LIMIT}&appid={API_KEY}";
        _mockApiService.GetAsync(url).Returns(jsonResponse);

        var results = await _geocodeApiService.GetCoordinatesByCityName(requestedCityName);
        var resultsList = results.ToList();

        Assert.That(resultsList, Has.Count.EqualTo(2));
        Assert.That(resultsList, Contains.Item(cities[1]));
        Assert.That(resultsList, Contains.Item(cities[0]));
    }

    [Test]
    public async Task GetCoordinatesByCityName_ReturnedListDoesntContainTheCityName_ThrowsNotFoundException()
    {
        // Create a list of Coordinates objects without the requested city name
        var cities = new List<City>
        {
            new City
                { Latitude = 34.0522, Longitude = -118.2437, Name = "notMatched2", State = "test", Country = "test" },
            new City
                { Latitude = 37.7749, Longitude = -122.4194, Name = "notMatched1", State = "test", Country = "test" }
        };

        // Serialize the list to a JSON string
        var jsonResponse = JsonSerializer.Serialize(cities);

        var requestedCityName = "testCity";
        var url =
            $"{BASE_URL}/direct?q={requestedCityName}&limit={LIMIT}&appid={API_KEY}";
        _mockApiService.GetAsync(url).Returns(jsonResponse);

        // Act & Assert
        Assert.ThrowsAsync<NotFoundException>(async () =>
            await _geocodeApiService.GetCoordinatesByCityName(requestedCityName));
    }
}