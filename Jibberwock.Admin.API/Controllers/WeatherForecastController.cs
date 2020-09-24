using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Jibberwock.Admin.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)],
                HttpHeaders = HttpContext.Request.Headers.Keys.Select(x => $"{x}: {string.Join("; ", HttpContext.Request.Headers[x].ToArray())}").ToArray()
            })
            .ToArray();
        }

        [Route("claims")]
        [HttpGet]
        [Authorize(AuthenticationSchemes = "EasyAuth")]
        public IActionResult GetClaims()
        {
            return Ok(User?.Claims.Select(x => new { x.Type, x.Value, x.Issuer, x.Properties }));
        }
    }
}
