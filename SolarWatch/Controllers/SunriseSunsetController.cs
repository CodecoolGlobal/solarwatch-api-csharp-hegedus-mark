using Microsoft.AspNetCore.Mvc;
using SolarWatch.DTOs;
using SolarWatch.Exceptions;
using SolarWatch.Models;
using SolarWatch.Services;

namespace SolarWatch.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SunriseSunsetController : ControllerBase
{
    private readonly IGeocodeApiService _geocodeApiService;
    private readonly ISunriseSunsetApiService _sunriseSunsetApiService;


    public SunriseSunsetController(IGeocodeApiService geocodeApiService,
        ISunriseSunsetApiService sunriseSunsetApiService)
    {
        _geocodeApiService = geocodeApiService;
        _sunriseSunsetApiService = sunriseSunsetApiService;
    }

    [HttpGet("{city}")]
    public async Task<ActionResult<List<SunriseSunset>>> GetSunriseSunsetByCity(string city)
    {
        try
        {
            var citiesWithSunriseSunset  = new List<CityWithSunriseSunsetResponse>();
            var coordinates = await _geocodeApiService.GetCoordinatesByCityName(city);
            foreach (var coordinate in coordinates)
            {
                var sunriseSunset = await _sunriseSunsetApiService.GetSunriseSunsetByCoordinates(coordinate);
                if (sunriseSunset is null) continue;
                
                var cityWithSunriseSunset = new CityWithSunriseSunsetResponse
                {
                    Name = coordinate.Name,
                    Country = coordinate.Country,
                    State = coordinate.State,
                    Latitude = coordinate.Latitude,
                    Longitude = coordinate.Longitude,
                    Sunrise = sunriseSunset.Sunrise,
                    Sunset = sunriseSunset.Sunset,
                };
            
                citiesWithSunriseSunset.Add(cityWithSunriseSunset);
            }

            return Ok(citiesWithSunriseSunset);
        }
        catch (ClientException e)
        {
            Console.WriteLine(e);
            return BadRequest(e.Message);
        }
        catch (NotFoundException e)
        {
            Console.WriteLine(e);
            return NotFound(e.Message);
        }
        catch (ExternalApiException e)
        {
            Console.WriteLine(e);
            return StatusCode(StatusCodes.Status502BadGateway, "Error communicating with the external API");
        }
        catch (InternalServerException e)
        {
            Console.WriteLine(e);
            return StatusCode(StatusCodes.Status500InternalServerError, "An Error occured, please try again later.");
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                "An Unexpected error occured, please try again later.");
        }
    }
}