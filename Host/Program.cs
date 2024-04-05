using Host.Configurations;
using Host.Extensions;
using Host.Infrastructure.BackgroundJobs;
using Host.Infrastructure.Caching;
using Host.Infrastructure.HttpClients;
using Host.Infrastructure.Logging;
using Host.Infrastructure.Middleware;
using Host.Infrastructure.Notifications;
using Host.Services;
using OpenTelemetry.Metrics;
using PostSharp.Patterns.Diagnostics;
using PostSharp.Patterns.Diagnostics.Backends.Serilog;
using Serilog;

namespace Host
{
    public class Program
    {
        public static void Main(string[] args)
        {           
            try
            {
                var builder = WebApplication.CreateBuilder(args);
                builder.AddConfigurations().RegisterSerilog();
                builder.Services.AddServices();
                //LoggingServices.DefaultBackend = new SerilogLoggingBackend(Log.Logger);
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
                if (app.Environment.IsDevelopment())
                {
                    app.UseSwagger();
                    app.UseSwaggerUI();
                }
                //app.UseSerilogRequestLogging();

                app.UseHangfireDashboard(builder.Configuration);

                app.UseExceptionMiddleware();

                app.UseHttpsRedirection();

                app.UseAuthorization();
                app.UseRouting();
                app.MapControllers();

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
