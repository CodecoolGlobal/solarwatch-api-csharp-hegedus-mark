using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SolarWatch.Data.Models;
using SolarWatch.Data.Repositories;
using SolarWatch.RequestsAndResponses;
using SolarWatch.Services;

namespace SolarWatch.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SunriseSunsetController : ControllerBase
{
    private readonly ICityDataService _cityDataService;
    private readonly ICityRepository _cityRepository;
    private readonly ILogger<SunriseSunsetController> _logger;

    public SunriseSunsetController(ICityDataService cityDataService, ILogger<SunriseSunsetController> logger,
        ICityRepository cityRepository)
    {
        _cityDataService = cityDataService;
        _logger = logger;
        _cityRepository = cityRepository;
    }

    [HttpGet("{cityName}"), Authorize(Roles = "User, Admin")]
    public async Task<ActionResult<List<SunriseSunset>>> GetSunriseSunsetByCity(string cityName)
    {
        var results = await _cityDataService.GetCityData(cityName);

        if (results.Count < 0)
        {
            return NotFound();
        }

        _logger.LogInformation($"Found {results.Count} sunrise sunsets");

        return Ok(results);
    }

    [HttpPost, Authorize(Roles = "Admin")]
    public IActionResult Post([FromBody] CityWithSunriseSunset cityWithSunriseSunset)
    {

        var cityModel = new City
        {
            Country = cityWithSunriseSunset.Country,
            Latitude = cityWithSunriseSunset.Latitude,
            Longitude = cityWithSunriseSunset.Longitude,
            Name = cityWithSunriseSunset.CityName,
            State = cityWithSunriseSunset.State,
            SunriseSunset = new SunriseSunset
            {
                Sunrise = cityWithSunriseSunset.Sunrise,
                Sunset = cityWithSunriseSunset.Sunset
            }
        };

        _cityRepository.Add(cityModel);
        return CreatedAtAction(nameof(Post), cityModel);
    }

    [HttpPut("{id:int}"), Authorize(Roles = "Admin")]
    public IActionResult Update([FromBody] CityWithSunriseSunset cityWithSunriseSunset, int id)
    {
        var city = _cityRepository.GetById(id);
        if (city == null)
        {
            return NotFound();
        }

        city.Country = cityWithSunriseSunset.Country;
        city.Latitude = cityWithSunriseSunset.Latitude;
        city.Longitude = cityWithSunriseSunset.Longitude;
        city.Name = cityWithSunriseSunset.CityName;
        city.State = cityWithSunriseSunset.State;
        city.SunriseSunset = new SunriseSunset
        {
            Sunset = cityWithSunriseSunset.Sunset,
            Sunrise = cityWithSunriseSunset.Sunrise
        };

        _cityRepository.Update(city);
        return NoContent();
    }

    [HttpDelete("{id:int}"), Authorize(Roles = "Admin")]
    public IActionResult Delete(int id)
    {
        var city = _cityRepository.GetById(id);
        if (city == null)
        {
            return NotFound();
        }

        _cityRepository.Delete(city);
        return NoContent();
    }
}