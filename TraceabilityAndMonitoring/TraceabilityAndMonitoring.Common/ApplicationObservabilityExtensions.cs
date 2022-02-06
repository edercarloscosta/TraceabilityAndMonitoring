using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace TraceabilityAndMonitoring.Common
{
    public static class ApplicationObservabilityExtensions
    {
         public static IServiceCollection AddApplicationObservabilityServices(
            this IServiceCollection services,
            string serviceName
        )
        {
            // Exporter: 
            // It provides metrics like CPU, memory, disk space, disk I/O usage
            services.Configure<KestrelServerOptions>(
                options =>
                {
                    options.AllowSynchronousIO = true;
                });
            services.AddMetrics();

            Activity.DefaultIdFormat = ActivityIdFormat.W3C;
            
            // Configure tracing
            services.AddOpenTelemetryTracing(builder =>
            {
                builder
                    .SetResourceBuilder(
                        ResourceBuilder
                            .CreateDefault()
                            .AddService($"service-{serviceName}"))
                    .AddAspNetCoreInstrumentation(options =>
                    {
                        options.Filter = (req) => !req.Request.Path.ToUriComponent()
                                                      .Contains("index.html", StringComparison.OrdinalIgnoreCase)
                                                  && !req.Request.Path.ToUriComponent().Contains("swagger",
                                                      StringComparison.OrdinalIgnoreCase);
                    })
                    .AddSource($"source-{serviceName}")
                    .AddHttpClientInstrumentation()
                    .AddJaegerExporter()
                    .AddOtlpExporter(opt 
                        => opt.Endpoint = new Uri("http://localhost:4317")); // todo: There is no yml file yet

            });

            // Configure metrics
            services.AddOpenTelemetryMetrics(builder =>
            {
                builder.AddHttpClientInstrumentation();
                builder.AddAspNetCoreInstrumentation();
                builder.AddMeter($"metrics-{serviceName}");
            });
            
            return services;
        }
    }
}