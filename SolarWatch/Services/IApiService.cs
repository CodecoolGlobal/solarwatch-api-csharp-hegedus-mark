namespace SolarWatch.Services;

public interface IApiService<T>
{
    Task<T> GetAsync(string url);
}