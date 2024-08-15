using Microsoft.AspNetCore.Mvc;
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
    public async Task<ActionResult<SunriseSunset>> GetSunriseSunsetByCity(string city)
    {
        try
        {
            var coordinates = await _geocodeApiService.GetCoordinatesByCityName(city);
            var sunriseSunset = await _sunriseSunsetApiService.GetSunriseSunsetByCoordinates(coordinates);

            return Ok(sunriseSunset);
        }
        catch (ClientException e)
        {
            Console.WriteLine(e);
            return BadRequest(e.Message);
        }
        catch (ExternalCustomException e)
        {
            Console.WriteLine(e);
            return StatusCode(StatusCodes.Status502BadGateway, "Error communicating with the external API");
        }
        catch (InternalServerException e)
        {
            Console.WriteLine(e);
            return StatusCode(StatusCodes.Status500InternalServerError, "An Error occured, please try again later.");
        }
    }
}