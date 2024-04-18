using Host.Integration.Configurations;
using Host.Integration.Logging;
using Host.Integration.Tracing;
using Microsoft.Extensions.Primitives;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Events;

//[assembly: LoggingAspect]
namespace Host.Integration
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //Log.Logger = new LoggerConfiguration()
            //    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            //    .Enrich.FromLogContext()
            //    .Enrich.WithCorrelationIdHeader("custom-correlation-id")
            //    .WriteTo.Console()
            //    .CreateBootstrapLogger();

            try
            {
                var builder = WebApplication.CreateBuilder(args);

                builder.AddConfigurations().RegisterSerilog();

                // Add services to the container.               
                builder.Services.AddControllers();
                // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSwaggerGen();

                builder.Services.AddOpenTelemetry()
                    .WithTracing(builder => builder
                    .AddSource(OrderTracingFactory.ActivityNameOrderSource)
                    .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(OrderTracingFactory.ServiceName))
                    .AddAspNetCoreInstrumentation(opt =>
                    {
                        opt.RecordException = true;
                        opt.EnrichWithHttpRequest = (activity, httpRequest) =>
                        {
                            httpRequest.HttpContext.Request.Headers.TryGetValue("X-Correlation-Id", out StringValues correlationId);
                            var traceId = httpRequest.HttpContext.TraceIdentifier;
                            activity.SetBaggage("Correlation-Id", correlationId.FirstOrDefault() ?? "default");
                            activity.SetBaggage("Trace-Id", traceId ?? "default");
                        };
                    })
                    //.SetSampler(new AlwaysOnSampler()) // https://github.com/open-telemetry/opentelemetry-dotnet/blob/main/docs/trace/customizing-the-sdk/README.md#samplers
                    .AddHttpClientInstrumentation()
                .AddConsoleExporter()
                .AddOtlpExporter(opts => { opts.Endpoint = new Uri("http://104.131.189.170:4317"); }));

                //builder.Services.AddOpenTelemetryTracing(builder.Configuration);

                var app = builder.Build();
                app.UseSerilogRequestLogging(options =>
                {
                    // Customize the message template
                    options.MessageTemplate = "Handled {RequestPath}";

                    // Emit debug-level events instead of the defaults
                    options.GetLevel = (httpContext, elapsed, ex) => LogEventLevel.Debug;

                    // Attach additional properties to the request completion event
                    options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
                    {
                        diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
                        diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
                    };
                });

                // Configure the HTTP request pipeline.
                app.UseSwagger();
                app.UseSwaggerUI();

                app.UseAuthorization();

                app.UseMiddleware<RequestContextLoggingMiddleware>();
                app.MapControllers();

                app.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }

        }
    }
}
