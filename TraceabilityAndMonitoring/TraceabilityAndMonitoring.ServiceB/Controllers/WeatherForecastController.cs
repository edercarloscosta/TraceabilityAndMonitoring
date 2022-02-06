using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace TraceabilityAndMonitoring.ServiceB.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly Activity _activity = Activity.Current;
        private static readonly string[] Summaries = new[]
        {
            "Freezing From Srv B", 
            "Bracing From Srv B", 
            "Chilly From Srv B", 
            "Cool From Srv B", 
            "Mild From Srv B", 
            "Warm From Srv B", 
            "Balmy From Srv B", 
            "Hot From Srv B", 
            "Sweltering From Srv B", 
            "Scorching From Srv B"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            _logger.LogInformation("[ServiceB::Get]");
            
            var rng = new Random();
            var resultServiceB = Enumerable.Range(1, 5).Select(index => new WeatherForecast
                {
                    Date = DateTime.Now.AddDays(index),
                    TemperatureC = rng.Next(-20, 55),
                    Summary = Summaries[rng.Next(Summaries.Length)]
                })
                .ToArray();
            
            if (_activity != null && _activity.IsAllDataRequested)
            {
                /*
                 * Important:
                 * If you are instrumenting functions with high-performance requirements,
                 * Activity.IsAllDataRequested is a hint that indicates whether any of the code listening to Activities
                 * intends to read auxiliary information such as Tags. If no listener will read it,
                 * then there is no need for the instrumented code to spend CPU cycles populating it.
                 *
                 * Reference:
                 * https://docs.microsoft.com/en-us/dotnet/api/system.diagnostics.activity.isalldatarequested?view=net-6.0#system-diagnostics-activity-isalldatarequested
                 * TODO: Identify if here at this step de "Instrumentation" needs to understand it depends on high processing 
                 */
                _activity?.SetTag("[ServiceB::Processing]", "Saying hello from service B processing a lot ;)");     
            }
            _activity?.SetTag("[ServiceB::Get]", "Saying hello from service B ;)");
            _activity?.AddEvent(new("[ServiceB::EventTriggered::WeatherForecast - Service B]", DateTimeOffset.Now));
            
            return resultServiceB;
        }
    }
}