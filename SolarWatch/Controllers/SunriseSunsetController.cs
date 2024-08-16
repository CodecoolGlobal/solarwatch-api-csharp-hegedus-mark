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
    public async Task<ActionResult<List<SunriseSunset>>> GetSunriseSunsetByCity(string city)
    {
        try
        {
            var cities = new List<SunriseSunset>();
            var coordinates = await _geocodeApiService.GetCoordinatesByCityName(city);
            foreach (var coordinate in coordinates)
            {
                var result = await _sunriseSunsetApiService.GetSunriseSunsetByCoordinates(coordinate);
                if (result is null) continue;
                cities.Add(result);
            }

            return Ok(cities);
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