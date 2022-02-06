using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace TraceabilityAndMonitoring.ServiceA.Http
{
    public class HttpRequestIntegration: IHttpRequestIntegration
    {
        private const string URL = "https://localhost:5002"; // Or container "service_b:5002";
        private readonly ILogger<HttpRequestIntegration> _logger;
        private readonly Activity _activity = Activity.Current;

        public HttpRequestIntegration(ILogger<HttpRequestIntegration> logger)
            => _logger = logger;
        
        public async Task<IEnumerable<WeatherForecast>> HttpClientRequest()
        {
            try
            {
                var httpClient = HttpClientFactory.Create();

                var httpResponseMessage = await httpClient.GetAsync($"{URL}/weatherforecast");

                if (httpResponseMessage.StatusCode != HttpStatusCode.OK) return null;
            
                var content = httpResponseMessage.Content;
            
                return await content.ReadAsAsync<IEnumerable<WeatherForecast>>();

            }
            catch (Exception e)
            {
                _logger.LogInformation("[Exception::HttpClientRequest]", e.Message);
                _activity?.SetTag("[Exception::HttpClientRequest]", e.Message);   
                
                throw new ArgumentNullException(nameof(HttpRequestIntegration));   
            }
            
        }

        public void TryHttpClientRequest() =>
            throw new ArgumentNullException(nameof(HttpRequestIntegration));

        public async void TryInnerHttpClientRequest()
        {
            var httpClient = HttpClientFactory.Create();
            await httpClient.GetAsync($"{URL}/weatherforecast/get_inner_errors");
        }
    }
}