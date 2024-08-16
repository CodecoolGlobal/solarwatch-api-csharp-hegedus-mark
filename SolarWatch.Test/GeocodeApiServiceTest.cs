using Microsoft.Extensions.Options;
using SolarWatch.Configuration;
using SolarWatch.Exceptions;
using SolarWatch.Models;
using SolarWatch.Services;
using JsonException = System.Text.Json.JsonException;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace SolarWatch.Test;

public class GeocodeApiServiceTest
{
    private GeocodeApiService _geocodeApiService;
    private IApiService<Coordinates> _mockApiService;
    private IOptions<ExternalApiSettings> _mockOptions;

    [SetUp]
    public void Setup()
    {
        // Mock the IApiService<Coordinates>
        _mockApiService = Substitute.For<IApiService<Coordinates>>();


        // Mock the configuration
        var mockSettings = new ExternalApiSettings
        {
            GeocodeBaseUrl = "https://api.geocode.com",
            GeocodeApiKey = "test_api_key"
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
        var expectedCoordinates = new List<Coordinates>
        {
            new() { Latitude = 51.5074, Longitude = -0.1278, Name = cityName }
        };
        var url = $"https://api.geocode.com/direct?q={cityName}&limit=1&appid=test_api_key";

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
        var url = $"https://api.geocode.com/direct?q={cityName}&limit=1&appid=test_api_key";
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
        var url = $"https://api.geocode.com/direct?q={cityName}&limit=1&appid=test_api_key";
        _mockApiService.GetAsync(url).Returns(response);

        Assert.ThrowsAsync<JsonException>(() => _geocodeApiService.GetCoordinatesByCityName(cityName));
    }

    [Test]
    public async Task GetCoordinatesByCityName_MultipleItems_ReturnsTheCityThatMatchesTheRequest()
    {
        // Create a list of Coordinates objects
        var cities = new List<Coordinates>
        {
            new Coordinates { Latitude = 40.7128, Longitude = -74.0060, Name = "testCity" },
            new Coordinates { Latitude = 34.0522, Longitude = -118.2437, Name = "testCityAlpha" },
            new Coordinates { Latitude = 37.7749, Longitude = -122.4194, Name = "testCityBeta" }
        };

        // Serialize the list to a JSON string
        var jsonResponse = JsonSerializer.Serialize(cities);

        var requestedCityName = "testCity";
        var url = $"https://api.geocode.com/direct?q={requestedCityName}&limit=1&appid=test_api_key";
        _mockApiService.GetAsync(url).Returns(jsonResponse);

        var results = await _geocodeApiService.GetCoordinatesByCityName(requestedCityName);
        var result = results.ToList()[0];

        Assert.That(result.Name, Is.EqualTo(requestedCityName));
    }

    [Test]
    public async Task GetCoordinatesByCityName_ReturnedListDoesntContainTheCityName_ThrowsNotFoundException()
    {
        // Create a list of Coordinates objects without the requested city name
        var cities = new List<Coordinates>
        {
            new Coordinates { Latitude = 34.0522, Longitude = -118.2437, Name = "testCityAlpha" },
            new Coordinates { Latitude = 37.7749, Longitude = -122.4194, Name = "testCityBeta" }
        };

        // Serialize the list to a JSON string
        var jsonResponse = JsonSerializer.Serialize(cities);

        var requestedCityName = "testCity";
        var url = $"https://api.geocode.com/direct?q={requestedCityName}&limit=1&appid=test_api_key";
        _mockApiService.GetAsync(url).Returns(jsonResponse);

        // Act & Assert
        Assert.ThrowsAsync<NotFoundException>(async () =>
            await _geocodeApiService.GetCoordinatesByCityName(requestedCityName));
    }
}