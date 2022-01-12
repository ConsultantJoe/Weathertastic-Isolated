using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Serilog.Events;
using System;
using System.Threading.Tasks;
using Weathertastic_Isolated.Temperature.Infrastructure.Services;

namespace Weathertastic_Isolated.Temperature.Api.Controllers
{
    /// <summary>
    /// Gets current tempature data from the weather service. 
    /// </summary>
    [ApiController]
    public class TemperatureController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IWeatherService _weatherService;
        public TemperatureController(ILogger logger, IWeatherService weatherService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _weatherService = weatherService ?? throw new ArgumentNullException(nameof(weatherService));
        }

        [HttpGet("api/v1/Temperature/GetTemperatureByCity/{cityName}")]
        public async Task<IActionResult> GetTemperatureByCity(string cityName)
        {
            try
            {
                var result = await _weatherService.GetTempatureByCity(cityName);
                if(result != null)
                {
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                _logger.Write(LogEventLevel.Error, $"TemperatureController::GetTemperatureByCity - An unexpected error occcurred. City: {cityName} Exception: {ex}");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }

            return NotFound();
        }
    }
}
