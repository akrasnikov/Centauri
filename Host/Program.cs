using Host.Configurations;
using Host.Events;
using Host.Infrastructure.BackgroundJobs;
using Host.Infrastructure.Caching;
using Host.Infrastructure.HttpClients;
using Host.Infrastructure.Logging;
using Host.Infrastructure.Middleware;
using Host.Infrastructure.Notifications;
using Host.Infrastructure.Tracing;
using Host.Services;
using OpenTelemetry.Metrics;
using PostSharp.Patterns.Diagnostics;
using PostSharp.Patterns.Diagnostics.Backends.Serilog;
using PostSharp.Patterns.Diagnostics.RecordBuilders;
using Serilog;


//[assembly: LogTrace(AttributeTargetElements = MulticastTargets.Method, AttributeTargetTypeAttributes = MulticastAttributes.Public, AttributeTargetMemberAttributes = MulticastAttributes.Public)]

namespace Host
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
