using System.Collections.Generic;
using System.Threading.Tasks;

namespace TraceabilityAndMonitoring.ServiceA.Http
{
    public interface IHttpRequestIntegration
    {
        Task<IEnumerable<WeatherForecast>> HttpClientRequest();
    }
}