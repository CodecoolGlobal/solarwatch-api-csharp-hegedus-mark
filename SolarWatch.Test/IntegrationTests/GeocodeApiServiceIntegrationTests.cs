using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using SolarWatch.Configuration;
using SolarWatch.Models;
using SolarWatch.Services;

namespace SolarWatch.Test.IntegrationTests;

public class GeocodeApiServiceIntegrationTests
{
    private GeocodeApiService _geocodeApiService;

    [SetUp]
    public void Setup()
    {
        // Set up the configuration to use user-secrets
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.Test.json", optional: false, reloadOnChange: true)
            .AddUserSecrets<GeocodeApiServiceIntegrationTests>()
            .Build();

        var settings = new ExternalApiSettings
        {
            GeocodeBaseUrl = configuration["GeocodeBaseUrl"], // Get base URL from appsettings.json or user-secrets
            GeocodeApiKey = configuration["GeocodeApiKey"] // Get API key from user-secrets
        };

        var apiConfig = new ApiServiceConfiguration { MaxRetries = 1, RetryDelayMilliseconds = 0 };

        var options = Options.Create(settings);
        var apiService = new ApiService<Coordinates>(new HttpClient(), apiConfig);

        _geocodeApiService = new GeocodeApiService(apiService, options);
    }
    
    [Test]
    [Category("Integration")]
    public async Task GetCoordinatesByCityName_ShouldReturnValidCoordinates()
    {
        // Arrange
        var cityName = "London";

        // Act
        var result = await _geocodeApiService.GetCoordinatesByCityName(cityName);

        // Assert
        Assert.IsNotNull(result);
        Assert.That(result.Latitude, Is.Not.EqualTo(null));
        Assert.That(result.Longitude, Is.Not.EqualTo(null));
    }
}