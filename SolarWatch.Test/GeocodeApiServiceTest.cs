using Microsoft.Extensions.Options;
using SolarWatch.Configuration;
using SolarWatch.Models;
using SolarWatch.Services;

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
    public async Task GetCoordinatesByCityName_ShouldCallApiServiceWithCorrectUrl()
    {
        // Arrange
        var cityName = "London";
        var expectedCoordinates = new Coordinates { Latitude = 51.5074, Longitude = -0.1278 };
        var url = "https://api.geocode.com/direct?q=London&limit=1&appid=test_api_key";
        
        _mockApiService.GetAsync(url).Returns(expectedCoordinates);

        // Act
        var result = await _geocodeApiService.GetCoordinatesByCityName(cityName);

        // Assert
        Assert.That(result, Is.EqualTo(expectedCoordinates));
        await _mockApiService.Received(1).GetAsync(url);
    }
}