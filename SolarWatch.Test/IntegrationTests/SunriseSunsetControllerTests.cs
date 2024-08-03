using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;

namespace SolarWatch.Test.IntegrationTests;

public class SunriseSunsetControllerTests
{
    private readonly WebApplicationFactory<Program> _factory = new();

    [SetUp]
    public void SetUp()
    {
    }
    
    [Test]
    public async Task GetSunriseSunsetByCity_RetrievesSunriseSunset()
    {
        var client = _factory.CreateClient();
        var city = "London";

        var response = await client.GetAsync($"api/SunriseSunset/{city}");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }
}