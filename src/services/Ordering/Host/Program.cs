using OpenTelemetry.Metrics;
using Ordering.Host.Configurations;
using Ordering.Host.Events;
using Ordering.Host.Infrastructure.BackgroundJobs;
using Ordering.Host.Infrastructure.Caching;
using Ordering.Host.Infrastructure.HttpClients;
using Ordering.Host.Infrastructure.Logging;
using Ordering.Host.Infrastructure.Middleware;
using Ordering.Host.Infrastructure.Notifications;
using Ordering.Host.Infrastructure.Tracing;
using Ordering.Host.Services;
using PostSharp.Patterns.Diagnostics;
using PostSharp.Patterns.Diagnostics.Backends.Serilog;
using PostSharp.Patterns.Diagnostics.RecordBuilders;
using Serilog;


//[assembly: LogTrace(AttributeTargetElements = MulticastTargets.Method, AttributeTargetTypeAttributes = MulticastAttributes.Public, AttributeTargetMemberAttributes = MulticastAttributes.Public)]

namespace Ordering.Host
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                var builder = WebApplication.CreateBuilder(args);
                builder.Logging.Configure(_ => _.ActivityTrackingOptions = ActivityTrackingOptions.TraceId | ActivityTrackingOptions.SpanId);

                builder.AddConfigurations().RegisterSerilog();
                builder.Services.AddControllers();
                builder.Services.AddServices();
                builder.Services.AddEvents();

                //var backend = new SerilogLoggingBackend(Log.Logger);
                //backend.Options.IncludeActivityExecutionTime = true;
                //backend.Options.IncludeExceptionDetails = true;
                //backend.Options.SemanticParametersTreatedSemantically = SemanticParameterKind.All;
                //backend.Options.IncludedSpecialProperties = SerilogSpecialProperties.All;
                //backend.Options.ContextIdGenerationStrategy = ContextIdGenerationStrategy.Hierarchical;
                //LoggingServices.DefaultBackend = backend;


                builder.Services.AddControllers();
                builder.Services.AddCaching(builder.Configuration);
                builder.Services.AddNotifications(builder.Configuration);
                builder.Services.AddExceptionMiddleware();

                builder.Services.AddBackgroundServices(builder.Configuration);
                builder.Services.AddIntegration(builder.Configuration);
                builder.Services.AddInstrumentation(builder.Configuration);

                // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSwaggerGen();

                var app = builder.Build();
                // Configure the HTTP request pipeline. 
                app.UseSwagger();
                app.UseSwaggerUI();
                app.UseHeaderPropagation();
                app.UseHangfireDashboard(builder.Configuration);
                app.UseExceptionMiddleware();

                app.UseHttpsRedirection();

                app.UseAuthorization();
                app.UseRouting();
                app.MapControllers();

                app.MapNotifications();

                app.UseOpenTelemetryPrometheusScrapingEndpoint();

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
