using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using SolarWatch.Configuration;
using SolarWatch.Exceptions;

namespace SolarWatch.Services;

public class ApiService<T> : IApiService<T>
{
    private readonly HttpClient _httpClient;
    private readonly int _maxRetries;
    private readonly int _retryDelayMilliseconds;

    public ApiService(HttpClient httpClient, ApiServiceConfiguration configuration)
    {
        _httpClient = httpClient;
        _maxRetries = configuration.MaxRetries;
        _retryDelayMilliseconds = configuration.RetryDelayMilliseconds;
    }

    public async Task<T> GetAsync(string url)
    {
        return await ExecuteWithRetry(async () =>
        {
            var response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                return await ProcessResponseContent(response);
            }

            HandleException(response);

            System.Diagnostics.Debug.Assert(false,
                "The HandleError method should always throw an exception if reached!");
            throw new CriticalServerLogicException("HandleError method did not throw an exception, when it should've");
        });
    }

    private async Task<T> ExecuteWithRetry(Func<Task<T>> action)
    {
        for (int retry = 0; retry < _maxRetries; retry++)
        {
            try
            {
                return await action();
            }
            catch (HttpRequestException e) when (ShouldAttemptRetry(retry))
            {
                await DelayRetry(retry);
            }
            catch (JsonException e)
            {
                throw new InternalServerException(
                    "Error occurred while processing the response from the external service.", e);
            }
            catch (HttpRequestException e)
            {
                throw new ExternalApiException("All retry attempts failed.", e);
            }
            catch (ApiException e)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new InternalServerException("Unexpected exception occured!", e);
            }
        }

        System.Diagnostics.Debug.Assert(false,
            "The retry loop should never complete without throwing an exception or returning a result.");
        throw new CriticalServerLogicException("The retry loop completed unexpectedly without returning a result.");
    }

    private static void HandleException(HttpResponseMessage response)
    {
        var statusCode = (int)response.StatusCode;

        throw statusCode switch
        {
            400 => new ClientException("The request was invalid. Please check the city name and try again."),

            404 => new ClientException("The requested resource could not be found."),

            >= 500 => new HttpRequestException(
                $"External Api Server Error: {response.StatusCode} {response.ReasonPhrase}"),

            _ => new ExternalApiException(
                $"Unexpected error occured: {response.StatusCode} {response.ReasonPhrase}")
        };
    }


    private async Task<T> ProcessResponseContent(HttpResponseMessage response)
    {
        var content = await response.Content.ReadAsStringAsync();

        if (string.IsNullOrWhiteSpace(content) || content == "[]")
        {
            throw new ClientException("The requested resource is not found, it is empty");
        }

        var firstItem = TryDeserializeArrayAndGetFirstItem(content);

        if (firstItem != null)
        {
            if (!IsValid(firstItem))
            {
                throw new ClientException("The requested resource contains invalid items.");
            }

            return firstItem;
        }

        var singleResult = JsonSerializer.Deserialize<T>(content);

        if (!IsValid(singleResult))
        {
            throw new ClientException("The requested resource contains invalid items.");
        }

        return singleResult!;
    }

    private T? TryDeserializeArrayAndGetFirstItem(string content)
    {
        List<T> arrayResult;
        try
        {
            arrayResult = JsonSerializer.Deserialize<List<T>>(content) ?? throw new JsonException();
        }
        catch (JsonException e)
        {
            return default;
        }

        return arrayResult is { Count: > 0 } ? arrayResult.First() : default;
    }


    private bool IsValid(T? obj)
    {
        if (obj == null)
        {
            return false;
        }


        var context = new ValidationContext(obj);
        var results = new List<ValidationResult>();
        bool isValid = Validator.TryValidateObject(obj, context, results, true);

        return isValid;
    }


    private bool ShouldAttemptRetry(int previousRetries)
    {
        return previousRetries < _maxRetries - 1;
    }

    private async Task DelayRetry(int retryCount)
    {
        // Exponential backoff
        var delay = Math.Pow(2, retryCount) * _retryDelayMilliseconds;
        await Task.Delay((int)delay);
    }
}