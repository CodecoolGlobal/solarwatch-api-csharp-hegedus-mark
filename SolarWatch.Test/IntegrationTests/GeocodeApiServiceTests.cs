using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using SolarWatch.Configuration;
using SolarWatch.Services;
namespace SolarWatch.Test.IntegrationTests;

[Category("IntegrationTests")]
public class GeocodeApiServiceTests
{
    private GeocodeApiService _geocodeApiService;

    [SetUp]
    public void Setup()
    {
        // Set up the configuration to use user-secrets
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.Development.json", optional: false, reloadOnChange: true)
            .AddUserSecrets<GeocodeApiServiceTests>()
            .Build();

        var settings = new ExternalApiSettings
        {
            GeocodeBaseUrl = configuration["GeocodeBaseUrl"], // Get base URL from appsettings.json or user-secrets
            GeocodeApiKey = configuration["GeocodeApiKey"] // Get API key from user-secrets
        };

        var apiConfig = new ApiServiceConfiguration { MaxRetries = 1, RetryDelayMilliseconds = 0 };

        var options = Options.Create(settings);
        var apiService = new ApiService(new HttpClient(), apiConfig);

        _geocodeApiService = new GeocodeApiService(apiService, options);
    }

    [Test]
    public async Task GetCoordinatesByCityName_ShouldReturnValidCoordinates()
    {
        // Arrange
        var cityName = "London";

        // Act
        var results = await _geocodeApiService.GetCoordinatesByCityName(cityName);
        var result = results.ToList()[0];

        // Assert
        Assert.IsNotNull(result);
        Assert.That(result.Latitude, Is.Not.EqualTo(null));
        Assert.That(result.Longitude, Is.Not.EqualTo(null));
    }
}