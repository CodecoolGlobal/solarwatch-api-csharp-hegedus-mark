using SolarWatch.Data.Models;
using SolarWatch.Data.Repositories;
using SolarWatch.DTOs;

namespace SolarWatch.Services;

public class CityDataService : ICityDataService
{
    private readonly ICityRepository _cityRepository;
    private readonly IGeocodeApiService _geocodeApiService;
    private readonly ISunriseSunsetApiService _sunriseSunsetApiService;

    public CityDataService(ICityRepository cityRepository, IGeocodeApiService geocodeApiService,
        ISunriseSunsetApiService sunriseSunsetApiService)
    {
        _cityRepository = cityRepository;
        _geocodeApiService = geocodeApiService;
        _sunriseSunsetApiService = sunriseSunsetApiService;
    }


    public async Task<List<CityWithSunriseSunsetResponse>> GetCityData(string cityName)
    {
        var cities = _cityRepository.GetByName(cityName).ToList();

        if (cities.Count > 0)
        {
            return MapMultipleCities(cities);
        }

        return await GetCitiesDataFromApi(cityName);
    }


    private async Task<List<CityWithSunriseSunsetResponse>> GetCitiesDataFromApi(string cityName)
    {
        var result = new List<City>();
        var cities = await _geocodeApiService.GetCoordinatesByCityName(cityName);

        foreach (var city in cities)
        {
            var updatedCity = await UpdateSunriseSunsetDataForCity(city);
            if (updatedCity == null) continue;
            _cityRepository.Add(updatedCity);
            result.Add(updatedCity);
        }

        return MapMultipleCities(result);
    }

    private async Task<City?> UpdateSunriseSunsetDataForCity(City city)
    {
        var sunriseSunset = await _sunriseSunsetApiService.GetSunriseSunsetByCoordinates(city);
        if (sunriseSunset is null) return null;

        city.SunriseSunset = sunriseSunset;
        return city;
    }


    private List<CityWithSunriseSunsetResponse> MapMultipleCities(IEnumerable<City> cities)
    {
        var citiesWithSunriseSunset = new List<CityWithSunriseSunsetResponse>();

        foreach (var city in cities)
        {
            citiesWithSunriseSunset.Add(MapCityToCityWithSunriseSunsetResponse(city));
        }

        return citiesWithSunriseSunset;
    }


    private CityWithSunriseSunsetResponse MapCityToCityWithSunriseSunsetResponse(City city)
    {
        return new CityWithSunriseSunsetResponse
        {
            Name = city.Name,
            Country = city.Country,
            State = city.State,
            Latitude = city.Latitude,
            Longitude = city.Longitude,
            Sunrise = city.SunriseSunset.Sunrise,
            Sunset = city.SunriseSunset.Sunset
        };
    }
}