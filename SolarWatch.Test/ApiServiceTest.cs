using System.Net;
using System.Text;
using System.Text.Json;
using RichardSzalay.MockHttp;
using SolarWatch.Configuration;
using SolarWatch.Exceptions;
using SolarWatch.Services;
using SolarWatch.Test.TestHelpers;

namespace SolarWatch.Test;

public class Tests
{
    private IApiService<ApiTestResponse> _apiService;
    private HttpClient _httpClient;
    private MockHttpMessageHandler _mockHttp;
    private ApiServiceConfiguration _configuration;

    [SetUp]
    public void Setup()
    {
        _configuration = new ApiServiceConfiguration { MaxRetries = 3, RetryDelayMilliseconds = 5 };

        _mockHttp = new MockHttpMessageHandler();
        _httpClient = _mockHttp.ToHttpClient();
        _apiService = new ApiService<ApiTestResponse>(_httpClient, _configuration);
    }

    [TearDown]
    public void TearDown()
    {
        _httpClient?.Dispose();
        _mockHttp?.Dispose();
    }


    [Test]
    public async Task GetAsync_ShouldReturnDeserializedContent_WhenResponseIsSuccessful()
    {
        var url = "http://example.com/api/resource";
        var expectedResponse = new ApiTestResponse { Name = "Test McGee" };
        _mockHttp.When(url)
            .Respond("application/json", JsonSerializer.Serialize(expectedResponse));

        var result = await _apiService.GetAsync(url);

        Assert.That(result, Is.Not.Null);
        Assert.That(expectedResponse.Name, Is.EqualTo(result.Name));
    }


    [Test]
    public void GetAsync_ShouldRetryAndThrowException_WhenApiReturnsServerError()
    {
        var url = "http://example.com/api/resource";
        var request = _mockHttp.When(url).Respond(HttpStatusCode.InternalServerError);


        Assert.ThrowsAsync<ExternalApiException>(async () => await _apiService.GetAsync(url));
        Assert.That(_mockHttp.GetMatchCount(request), Is.EqualTo(3));
    }

    [Test]
    public async Task GetAsync_ShouldRetryAndReturnResponse_WhenApiRecovers()
    {
        var url = "http://example.com/api/resource";
        var expectedResponse = new ApiTestResponse { Name = "Test McGee" };

        // Define a list of responses
        var responses = new List<HttpResponseMessage>
        {
            new(HttpStatusCode.InternalServerError),
            new(HttpStatusCode.InternalServerError),
            new(HttpStatusCode.OK)
            {
                Content = new StringContent(
                    JsonSerializer.Serialize(expectedResponse),
                    System.Text.Encoding.UTF8,
                    "application/json")
            }
        };
        int responseIndex = 0;
        var request = _mockHttp.When(url)
            .Respond(req =>
            {
                var response = responses[responseIndex];
                responseIndex = (responseIndex + 1) % responses.Count;
                return response;
            });

        var result = await _apiService.GetAsync(url);

        Assert.That(_mockHttp.GetMatchCount(request), Is.EqualTo(3));
        Assert.That(result.Name, Is.EqualTo(expectedResponse.Name));
    }


    [TestCase(HttpStatusCode.BadRequest, typeof(ClientException))]
    [TestCase(HttpStatusCode.NotFound, typeof(ClientException))]
    [TestCase(HttpStatusCode.Forbidden, typeof(ExternalApiException))]
    public void GetAsync_ShouldThrowExpectedException_ForClientErrors(HttpStatusCode statusCode,
        Type expectedException)
    {
        var url = "http://example.com/api/resource";
        var request = _mockHttp.When(url).Respond(statusCode);

        Assert.ThrowsAsync(expectedException, async () => await _apiService.GetAsync(url));
        Assert.That(_mockHttp.GetMatchCount(request), Is.EqualTo(1));
    }

    [Test]
    public void GetAsync_ShouldThrowInternalServerException_WhenResponseHasInvalidJson()
    {
        var url = "http://example.com/api/resource";
        var wrongJsonFormat = new StringContent("asd", Encoding.UTF8, "application/json");
        var request = _mockHttp.When(url).Respond(wrongJsonFormat);

        Assert.ThrowsAsync<InternalServerException>((() => _apiService.GetAsync(url)));
    }

    [TestCase("{}")]
    [TestCase("{\"Test\":\"Test\"}")]
    [TestCase("[{\"Test\":\"Test\"}, {\"Test\":\"Test2\"}]")]
    [TestCase("[]")]
    [TestCase("")]
    public void GetAsync_ShouldThrowClientException_WhenResponseIsInvalid(string content)
    {
        var url = "http://example.com/api/resource";
        var empty = new StringContent(content, Encoding.UTF8, "application/json");

        var request = _mockHttp.When(url).Respond(empty);

        Assert.ThrowsAsync<ClientException>(() => _apiService.GetAsync(url));
    }

    [Test]
    public async Task GetAsync_ShouldReturnFirstItem_WhenResponseIsArray()
    {
        var url = "http://example.com/api/resource";
        var responseArray = new ApiTestResponse[]
        {
            new() { Name = "Test1" },
            new() { Name = "Test2" }
        };
        var request = _mockHttp.When(url).Respond("application/json", JsonSerializer.Serialize(responseArray));

        var result = await _apiService.GetAsync(url);

        Assert.That(result.Name, Is.EqualTo("Test1"));
    }

    [Test]
    public async Task GetAsync_ShouldReturnFullArray_WhenApiServiceTypeIsArray()
    {
        var arrayApiService = new ApiService<ApiTestResponse[]>(_httpClient, _configuration);
        var url = "http://example.com/api/resource";
        var responseArray = new ApiTestResponse[]
        {
            new() { Name = "Test1" },
            new() { Name = "Test2" }
        };
        var request = _mockHttp.When(url).Respond("application/json", JsonSerializer.Serialize(responseArray));

        var result = await arrayApiService.GetAsync(url);

        Assert.That(result[0].Name, Is.EqualTo("Test1"));
        Assert.That(result[1].Name, Is.EqualTo("Test2"));
    }
}