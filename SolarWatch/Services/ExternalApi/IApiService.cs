namespace SolarWatch.Services;

public interface IApiService
{
    Task<string> GetAsync(string url);
}