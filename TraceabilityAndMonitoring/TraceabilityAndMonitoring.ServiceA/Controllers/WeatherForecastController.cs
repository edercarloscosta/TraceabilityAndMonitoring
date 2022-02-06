using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TraceabilityAndMonitoring.ServiceA.Http;

namespace TraceabilityAndMonitoring.ServiceA.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly Activity _activity = Activity.Current;
        private static readonly string[] Summaries = new[]
        {
            "Freezing From Srv A", 
            "Bracing From Srv A", 
            "Chilly From Srv A", 
            "Cool From Srv A", 
            "Mild From Srv A", 
            "Warm From Srv A", 
            "Balmy From Srv A", 
            "Hot From Srv A", 
            "Sweltering From Srv A", 
            "Scorching From Srv A"
        };
        
        private readonly IHttpRequestIntegration _httpClientRequest;
        
        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(
            ILogger<WeatherForecastController> logger,
            IHttpRequestIntegration httpClientRequest)
        {
            _logger = logger;
            _httpClientRequest = httpClientRequest;
        }

        [HttpGet]
        public async Task<IEnumerable<WeatherForecast>> Get()
        {
            _logger.LogInformation("[ServiceA::Get]");
            _activity?.SetTag("[ServiceA::Get]",  "Saying hello from service A ;)");
            
            var rng = new Random();
            var resultServiceA = Enumerable.Range(1, 7).Select(index => new WeatherForecast
                {
                    Date = DateTime.Now.AddDays(index),
                    TemperatureC = rng.Next(-20, 55),
                    Summary = Summaries[rng.Next(Summaries.Length)]
                })
                .ToArray();
            
            var httpClient = await _httpClientRequest.HttpClientRequest();
            var result = resultServiceA.Concat(httpClient);

            // It's a Event mindset properly and it's suitable for recording modest number of events. 
            _activity?.AddEvent(new("[ServiceA::EventTriggered::WeatherForecast - Service A]", DateTimeOffset.Now));
            return result;
        }
    }
}