using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SolarWatch.Configuration; // Add this using directive
using SolarWatch.Data.Models;
using SolarWatch.Exceptions;
using SolarWatch.Services;

namespace SolarWatch.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SunriseSunsetController : ControllerBase
    {
        private readonly ICityDataService _cityDataService;
        private readonly ILogger<SunriseSunsetController> _logger;

        public SunriseSunsetController(ICityDataService cityDataService, ILogger<SunriseSunsetController> logger)
        {
            _cityDataService = cityDataService;
            _logger = logger;
        }

        [HttpGet("{cityName}"), Authorize(Roles = "User, Admin")]
        public async Task<ActionResult<List<SunriseSunset>>> GetSunriseSunsetByCity(string cityName)
        {
            Debug.WriteLine(this.HttpContext.User.Identity.Name);
            try
            {
                var results = await _cityDataService.GetCityData(cityName);

                if (results.Count < 0)
                {
                    return NotFound();
                }

                _logger.LogInformation($"Found {results.Count} sunrise sunsets");

                return Ok(results);
            }
            catch (ClientException e)
            {
                _logger.LogError(e, "ClientException occurred");
                return BadRequest(e.Message);
            }
            catch (NotFoundException e)
            {
                _logger.LogError(e, "NotFoundException occurred");
                return NotFound(e.Message);
            }
            catch (ExternalApiException e)
            {
                _logger.LogError(e, "ExternalApiException occurred");
                return StatusCode(StatusCodes.Status502BadGateway, "Error communicating with the external API");
            }
            catch (InternalServerException e)
            {
                _logger.LogError(e, "InternalServerException occurred");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "An Error occurred, please try again later.");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An unexpected error occurred");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "An Unexpected error occurred, please try again later.");
            }
        }
    }
}