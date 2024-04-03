using Host.Configurations;
using Host.Extensions;
using Host.Infrastructure.Logging.Serilog;
using OpenTelemetry.Metrics;
using PostSharp.Patterns.Diagnostics;
using PostSharp.Patterns.Diagnostics.Backends.Serilog;
using Serilog;

[assembly: Log]
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
                //LoggingServices.DefaultBackend = new SerilogLoggingBackend(Log.Logger);
                builder.Services.AddControllers();
                builder.Services.AddServices();
                builder.Services.AddBackgroundService();
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

                //app.UseHangfireDashboard(String.Empty);

                app.UseErrorHandlingMiddleware();

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
