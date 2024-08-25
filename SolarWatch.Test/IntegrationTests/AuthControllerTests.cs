using System.Net;
using System.Net.Http.Json;
using System.Text;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using SolarWatch.RequestsAndResponses.Auth;
using SolarWatch.Test.TestHelpers;

namespace SolarWatch.Test.IntegrationTests;

[Category("IntegrationTests")]
public class AuthControllerTests
{
    private HttpClient _client;
    private SolarWatchWebApplicationFactory _factory;

    [SetUp]
    public void Setup()
    {
        _factory = new SolarWatchWebApplicationFactory();
        _client = _factory.CreateClient();
        
    }

    [TearDown]
    public void TearDown()
    {
        _factory.Dispose();
        _client.Dispose();
    }
    
    
    [Test]
    public async Task Register_WithValidData_ReturnsOkAndToken()
    {
        var testEmail = "test@email.com";
        var testUsername = "test";
        var password = "password";
        
        // Arrange
        var request = new RegistrationRequest(testEmail, password, testUsername);

        var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/Auth/Register", content);
        response.EnsureSuccessStatusCode(); // This will throw if the status code is not 2xx

        var jsonResponse = await response.Content.ReadAsStringAsync();
        var (expectedEmail, expectedUsername, token) = JsonConvert.DeserializeObject<AuthResponse>(jsonResponse);

        // Assert
        Assert.That(expectedEmail, Is.EqualTo(testEmail));
        Assert.That(string.IsNullOrEmpty(token), Is.False);
    }
    
    [Test]
    public async Task Login_WithValidCredentials_ReturnsSuccess()
    {
        // Arrange
        var request = new
        {
            Email = "testuser@example.com",
            Password = "Test@1234"
        };
            
        var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
        // Act
        var response = await _client.PostAsync("/api/auth/login", content);

        // Assert
        response.EnsureSuccessStatusCode();
        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.IsNotEmpty(responseContent);
        // Add more assertions based on your response structure
    }

    [Test]
    public async Task Login_WithInvalidCredentials_ReturnsUnauthorized()
    {
        // Arrange
        var request = new
        {
            Email = "testuser@example.com",
            Password = "WrongPassword"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", request);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }
}