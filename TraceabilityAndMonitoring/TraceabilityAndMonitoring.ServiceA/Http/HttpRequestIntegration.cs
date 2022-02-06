using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace TraceabilityAndMonitoring.ServiceA.Http
{
    public class HttpRequestIntegration: IHttpRequestIntegration
    {
        private const string URL = "https://localhost:5002";
        
        public async Task<IEnumerable<WeatherForecast>> HttpClientRequest()
        {
            var httpClient = HttpClientFactory.Create();

            var httpResponseMessage = await httpClient.GetAsync($"{URL}/weatherforecast");

            if (httpResponseMessage.StatusCode != HttpStatusCode.OK) return null;
            
            var content = httpResponseMessage.Content;
            
            return await content.ReadAsAsync<IEnumerable<WeatherForecast>>();
        }
    }
}