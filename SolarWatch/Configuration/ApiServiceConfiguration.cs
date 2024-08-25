namespace SolarWatch.Configuration;

public class ApiServiceConfiguration
{
    public int MaxRetries { get; init; } = 3; // Default value
    public int RetryDelayMilliseconds { get; init; } = 1000; // Default value
}