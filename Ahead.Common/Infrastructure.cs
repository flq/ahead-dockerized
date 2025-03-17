using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace Ahead.Common;

public static class Infrastructure
{
    public static IHostApplicationBuilder ConfigureOpenTelemetry(this IHostApplicationBuilder builder)
    {
        /*
         * https://learn.microsoft.com/en-us/dotnet/core/diagnostics/observability-otlp-example
         * OTEL Export is necessary to bring the dashboard to life with tracing etc.
         *
         * Tracing stuff yourself
         * https://github.com/open-telemetry/opentelemetry-dotnet/tree/main/src/OpenTelemetry.Api#instrumenting-a-libraryapplication-with-net-activity-api
         * 
         */
        builder.Logging.AddOpenTelemetry(logging =>
        {
            logging.IncludeFormattedMessage = true;
            logging.IncludeScopes = true;
        });

        var otel = builder.Services.AddOpenTelemetry()
            .WithMetrics(metrics =>
            {
                metrics
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation();
            })
            .WithTracing(tracing =>
            {
                tracing
                    .AddSource(builder.Environment.ApplicationName, OTelUtilities.MessagingActivitySource.Name)
                    .AddAspNetCoreInstrumentation(trace =>
                        // Don't trace requests to the health endpoint to avoid filling the dashboard with noise
                        trace.Filter = httpContext =>
                            !(httpContext.Request.Path.StartsWithSegments("/health")
                              || httpContext.Request.Path.StartsWithSegments("/alive"))
                    )
                    .AddHttpClientInstrumentation();
            });
        
        var useOtlpExporter = !string.IsNullOrWhiteSpace(builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]);
        if (useOtlpExporter)
        {
            otel.UseOtlpExporter();
        }

        return builder;
    }
}