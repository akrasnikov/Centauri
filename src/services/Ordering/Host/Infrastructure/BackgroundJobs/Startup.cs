using Hangfire;
using Hangfire.Console;
using Hangfire.Console.Extensions;
using Hangfire.InMemory;
using Ordering.Host.Exceptions;
using Ordering.Host.Infrastructure.BackgroundJobs;
using Serilog;
using ILogger = Serilog.ILogger;

namespace Ordering.Host.Infrastructure.BackgroundJobs;

internal static class Startup
{
    private static readonly ILogger _logger = Log.ForContext(typeof(Startup));

    internal static IServiceCollection AddBackgroundServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddHangfireServer(options => config.GetSection("HangfireSettings:Server").Bind(options));
        services.AddHangfireConsoleExtensions();

        var storageSettings = config.GetSection("HangfireSettings:Storage").Get<HangfireStorageSettings>() ?? throw new ExtensionException("Hangfire Storage Provider is not configured.");

        if (string.IsNullOrEmpty(storageSettings.StorageProvider))
            throw new ExtensionException("Hangfire Storage Provider is not configured.");
        if (string.IsNullOrEmpty(storageSettings.ConnectionString))
            throw new ExtensionException("Hangfire Storage Provider ConnectionString is not configured.");

        _logger.Information($"Hangfire: Current Storage Provider : {storageSettings.StorageProvider}");

        services.AddHangfire((provider, options) => options
            .UseIgnoredAssemblyVersionTypeResolver()
            .UseInMemoryStorage(new InMemoryStorageOptions
            {
                MaxExpirationTime = TimeSpan.FromHours(6)
            })
            .UseFilter(new LogJobFilter())
            .UseConsole());

        return services;
    }

    internal static IApplicationBuilder UseHangfireDashboard(this IApplicationBuilder app, IConfiguration config)
    {
        var dashboardOptions = config.GetSection("HangfireSettings:Dashboard").Get<DashboardOptions>() ?? throw new ExtensionException("Hangfire Dashboard is not configured.");
        //dashboardOptions.Authorization = new[]
        //{
        //   new HangfireCustomBasicAuthenticationFilter
        //   {
        //        User = config.GetSection("HangfireSettings:Credentials:User").Value!,
        //        Pass = config.GetSection("HangfireSettings:Credentials:Password").Value!
        //   }
        //};

        return app.UseHangfireDashboard(config["HangfireSettings:Route"], dashboardOptions);
    }
}