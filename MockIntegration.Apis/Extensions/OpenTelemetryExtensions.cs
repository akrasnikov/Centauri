using Host.Integration.Services;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Host.Integration.Extensions
{
    public static class OpenTelemetryExtensions
    {
        public static void AddOpenTelemetryTracing(this IServiceCollection services, IConfiguration configuration)
        {

            ActivityProvider.Create();
            services.AddOpenTelemetry().WithTracing(options =>
            {
                options.AddSource(ActivityProvider.ActivityName)
                    .AddSource(ActivityProvider.ListenerName)
                    .ConfigureResource(resource =>
                    {
                        resource.AddService(ActivityProvider.ServiceName,
                            serviceVersion: ActivityProvider.ServiceVersion);
                    });

                options.AddAspNetCoreInstrumentation(o =>
                {
                    // to trace only api requests
                    o.Filter = (context) => !string.IsNullOrEmpty(context.Request.Path.Value) && context.Request.Path.Value.Contains("Api", StringComparison.InvariantCulture);

                    // enrich activity with http request and response
                    o.EnrichWithHttpRequest = (activity, httpRequest) => { activity.SetTag("requestProtocol", httpRequest.Protocol); };
                    o.EnrichWithHttpResponse = (activity, httpResponse) => { activity.SetTag("responseLength", httpResponse.ContentLength); };

                    // automatically sets Activity Status to Error if an unhandled exception is thrown
                    o.RecordException = true;
                    o.EnrichWithException = (activity, exception) =>
                    {
                        activity.SetTag("exceptionType", exception.GetType().ToString());
                        activity.SetTag("stackTrace", exception.StackTrace);
                    };
                });
                options.AddConsoleExporter();
                options.AddOtlpExporter();
            });
        }
    }
}
