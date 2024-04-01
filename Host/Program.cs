using Host.Extensions;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;

namespace Host
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
          
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
            //app.UseHangfireDashboard(String.Empty);

            app.UseErrorHandlingMiddleware();

            app.UseHttpsRedirection();

            app.UseAuthorization();
            app.UseRouting();
            app.MapControllers();
           
            app.UseOpenTelemetryPrometheusScrapingEndpoint();
            app.Run();
        }
    }
}
