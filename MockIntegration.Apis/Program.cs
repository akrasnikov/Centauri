using Host.Integration.Services;
using Host.Integration.Extensions;
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
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateBootstrapLogger();

            try
            {
                var builder = WebApplication.CreateBuilder(args);
                //builder.Services.AddSerilog();
                builder.Host.UseSerilog(SeriLogger.Configure);
                // Add services to the container.
                builder.Services.AddTransient<IDummyClass, DummyClass>();
                builder.Services.AddControllers();
                // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSwaggerGen();

                var openTelemetryBuilder =
                    builder.Services.AddOpenTelemetry()
                    .ConfigureResource(resource => resource.AddService(ActivityProvider.ServiceName));

                openTelemetryBuilder
                    .WithTracing(tracerProviderBuilder =>
                    tracerProviderBuilder
                    .AddSource(ActivityProvider.ActivityName)
                    .AddAspNetCoreInstrumentation(options => options.RecordException = true)
                    .SetSampler(new AlwaysOnSampler()) // https://github.com/open-telemetry/opentelemetry-dotnet/blob/main/docs/trace/customizing-the-sdk/README.md#samplers
                    .AddHttpClientInstrumentation()
                .AddConsoleExporter()
                // 4317 // add the OpenTelemetry.Exporter.OpenTelemetryProtocol nuget package
                .AddOtlpExporter());

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
                if (app.Environment.IsDevelopment())
                {
                    app.UseSwagger();
                    app.UseSwaggerUI();
                }

                app.UseAuthorization();


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
