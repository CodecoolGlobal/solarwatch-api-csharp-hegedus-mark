using Microsoft.Extensions.Logging;
using SolarWatch.Data.Models;
using SolarWatch.Data.Repositories;
using SolarWatch.RequestsAndResponses;

namespace SolarWatch.Services
{
    public class CityDataService : ICityDataService
    {
        private readonly ICityRepository _cityRepository;
        private readonly IGeocodeApiService _geocodeApiService;
        private readonly ISunriseSunsetApiService _sunriseSunsetApiService;
        private readonly ILogger<CityDataService> _logger;

        public CityDataService(
            ICityRepository cityRepository,
            IGeocodeApiService geocodeApiService,
            ISunriseSunsetApiService sunriseSunsetApiService,
            ILogger<CityDataService> logger)
        {
            _cityRepository = cityRepository;
            _geocodeApiService = geocodeApiService;
            _sunriseSunsetApiService = sunriseSunsetApiService;
            _logger = logger;
        }

        public async Task<List<CityWithSunriseSunsetResponse>> GetCityData(string cityName)
        {
            _logger.LogInformation("GetCityData called with cityName: {CityName}", cityName);

            var cities = _cityRepository.GetByName(cityName).ToList();

            if (cities.Count > 0)
            {
                _logger.LogInformation("Cities found in repository for cityName: {CityName}", cityName);
                return MapMultipleCities(cities);
            }

            _logger.LogInformation("No cities found in repository, fetching data from API for cityName: {CityName}", cityName);
            return await GetCitiesDataFromApi(cityName);
        }

        private async Task<List<CityWithSunriseSunsetResponse>> GetCitiesDataFromApi(string cityName)
        {
            _logger.LogInformation("GetCitiesDataFromApi called with cityName: {CityName}", cityName);
            var result = new List<City>();
            var cities = await _geocodeApiService.GetCoordinatesByCityName(cityName);

            _logger.LogInformation("Coordinates fetched from API for cityName: {CityName}", cityName);

            foreach (var city in cities)
            {
                _logger.LogInformation("Updating sunrise and sunset data for city: {CityName}", city.Name);
                var updatedCity = await UpdateSunriseSunsetDataForCity(city);
                if (updatedCity == null)
                {
                    _logger.LogWarning("No sunrise/sunset data found for city: {CityName}", city.Name);
                    continue;
                }

                _cityRepository.Add(updatedCity);
                result.Add(updatedCity);
            }

            _logger.LogInformation("Data from API processed and added to repository for cityName: {CityName}", cityName);
            return MapMultipleCities(result);
        }

        private async Task<City?> UpdateSunriseSunsetDataForCity(City city)
        {
            _logger.LogInformation("Fetching sunrise and sunset data for city: {CityName}", city.Name);
            var sunriseSunset = await _sunriseSunsetApiService.GetSunriseSunsetByCoordinates(city);
            if (sunriseSunset is null)
            {
                _logger.LogWarning("No sunrise/sunset data returned from API for city: {CityName}", city.Name);
                return null;
            }

            city.SunriseSunset = sunriseSunset;
            _logger.LogInformation("Sunrise and sunset data updated for city: {CityName}", city.Name);
            return city;
        }

        private List<CityWithSunriseSunsetResponse> MapMultipleCities(IEnumerable<City> cities)
        {
            _logger.LogInformation("Mapping multiple cities to response DTOs");
            var citiesWithSunriseSunset = new List<CityWithSunriseSunsetResponse>();

            foreach (var city in cities)
            {
                citiesWithSunriseSunset.Add(MapCityToCityWithSunriseSunsetResponse(city));
            }

            _logger.LogInformation("Mapped {CityCount} cities to response DTOs", citiesWithSunriseSunset.Count);
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
}
