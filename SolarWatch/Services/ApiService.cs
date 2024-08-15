using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using SolarWatch.Configuration;
using SolarWatch.Exceptions;

//TODO: Remove validation from here and put it in the actual ApiService classes, this should only send a JSON string,
//The only thing this should check is if the string is an empty array, object or string. that's it.

namespace SolarWatch.Services;

/// <summary>
/// Provides methods to interact with an external API using an HttpClient.
/// Includes retry logic for transient failures and error handling for various response statuses.
/// </summary>
/// <typeparam name="T">The type of the object to be retrieved and deserialized from the API.</typeparam>
public class ApiService<T> : IApiService<T>
{
    private readonly HttpClient _httpClient;
    private readonly int _maxRetries;
    private readonly int _retryDelayMilliseconds;

    /// <summary>
    /// Constructs an instance of the ApiService with the provided HttpClient and configuration.
    /// </summary>
    /// <param name="httpClient">The HttpClient used to send HTTP requests.</param>
    /// <param name="configuration">Configuration settings for retries and delays.</param>
    public ApiService(HttpClient httpClient, ApiServiceConfiguration configuration)
    {
        _httpClient = httpClient;
        _maxRetries = configuration.MaxRetries;
        _retryDelayMilliseconds = configuration.RetryDelayMilliseconds;
    }

    /// <summary>
    /// Sends a GET request to the specified URL and returns the deserialized response.
    /// Retries on failure according to the configured retry policy.
    /// </summary>
    /// <param name="url">The URL to send the GET request to.</param>
    /// <returns>The deserialized response of type T.</returns>
    /// <exception cref="CriticalServerLogicException">Thrown when an unexpected exception occurs during processing.</exception>
    public async Task<string> GetAsync(string url)
    {
        return await ExecuteWithRetry(async () =>
        {
            var response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                return await GetJsonString(response);
            }

            HandleException(response);

            System.Diagnostics.Debug.Assert(false,
                "The HandleError method should always throw an exception if reached!");
            throw new CriticalServerLogicException("HandleError method did not throw an exception, when it should've");
        });
    }

    /// <summary>
    /// Executes a given asynchronous action with retry logic.
    /// Retries based on the configured maximum retries and handles exceptions accordingly.
    /// </summary>
    /// <param name="action">The asynchronous action to execute.</param>
    /// <returns>The result of the action.</returns>
    /// <exception cref="InternalServerException">Thrown when a JSON processing error occurs.</exception>
    /// <exception cref="ExternalApiException">Thrown when all retry attempts fail.</exception>
    /// <exception cref="CriticalServerLogicException">Thrown when the retry loop completes unexpectedly.</exception>
    private async Task<string> ExecuteWithRetry(Func<Task<string>> action)
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

    /// <summary>
    /// Handles HTTP response errors by throwing appropriate exceptions based on the status code.
    /// </summary>
    /// <param name="response">The HTTP response message containing the error details.</param>
    /// <exception cref="ClientException">Thrown for client-side errors (e.g., 400, 404).</exception>
    /// <exception cref="HttpRequestException">Thrown for server-side errors (e.g., 500 and above).</exception>
    /// <exception cref="ExternalApiException">Thrown for unexpected errors not covered by specific exceptions.</exception>
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

    private async Task<string> GetJsonString(HttpResponseMessage response)
    {
        var content = await response.Content.ReadAsStringAsync();
        return content;
    }

    /// <summary>
    /// Processes the content of an HTTP response, deserializing it and validating the result.
    /// </summary>
    /// <param name="response">The HTTP response message containing the content to process.</param>
    /// <returns>The deserialized and validated result of type T.</returns>
    /// <exception cref="ClientException">Thrown if the response content is invalid or empty.</exception>
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

    /// <summary>
    /// Tries to deserialize the response content as a list and returns the first item if available.
    /// </summary>
    /// <param name="content">The response content to deserialize.</param>
    /// <returns>The first item of the deserialized list or default if deserialization fails or the list is empty.</returns>
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

    /// <summary>
    /// Validates an object using data annotations.
    /// </summary>
    /// <param name="obj">The object to validate.</param>
    /// <returns>True if the object is valid; otherwise, false.</returns>
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

    /// <summary>
    /// Determines if a retry should be attempted based on the number of previous retries.
    /// </summary>
    /// <param name="previousRetries">The number of retries attempted so far.</param>
    /// <returns>True if more retries should be attempted; otherwise, false.</returns>
    private bool ShouldAttemptRetry(int previousRetries)
    {
        return previousRetries < _maxRetries - 1;
    }

    /// <summary>
    /// Applies an exponential backoff delay between retry attempts.
    /// </summary>
    /// <param name="retryCount">The number of previous retries.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    private async Task DelayRetry(int retryCount)
    {
        // Exponential backoff
        var delay = Math.Pow(2, retryCount) * _retryDelayMilliseconds;
        await Task.Delay((int)delay);
    }
}