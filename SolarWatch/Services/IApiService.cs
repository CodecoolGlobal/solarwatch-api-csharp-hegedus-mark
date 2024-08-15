namespace SolarWatch.Services;

public interface IApiService<T>
{
    Task<string> GetAsync(string url);
}