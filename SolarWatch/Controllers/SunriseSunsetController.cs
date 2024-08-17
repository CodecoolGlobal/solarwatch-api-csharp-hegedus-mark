using Microsoft.AspNetCore.Mvc;
using SolarWatch.Data.Models;
using SolarWatch.DTOs;
using SolarWatch.Exceptions;
using SolarWatch.Services;

namespace SolarWatch.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SunriseSunsetController : ControllerBase
{
    private readonly ICityDataService _cityDataService;


    public SunriseSunsetController(ICityDataService cityDataService)
    {
        _cityDataService = cityDataService;
    }

    [HttpGet("{cityName}")]
    public async Task<ActionResult<List<SunriseSunset>>> GetSunriseSunsetByCity(string cityName)
    {
        try
        {
            var results = await _cityDataService.GetCityData(cityName);

            if (results.Count < 0)
            {
                return NotFound();
            }
            
            return Ok(results);
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