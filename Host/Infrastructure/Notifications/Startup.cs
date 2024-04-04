using Host.Exceptions;
using Serilog;
using ILogger = Serilog.ILogger;

namespace Host.Infrastructure.Notifications
{
    internal static class Startup
    {
        internal static IServiceCollection AddNotifications(this IServiceCollection services, IConfiguration config)
        {
            ILogger logger = Log.ForContext(typeof(Startup));

            var signalRSettings = config.GetSection(nameof(SignalRSettings)).Get<SignalRSettings>() ?? throw new ExtensionException("SignalR is not configured."); ;

            if (!signalRSettings.UseBackplane)
            {
                services.AddSignalR();
            }
            else
            {
            //    var backplaneSettings = config.GetSection("SignalRSettings:Backplane").Get<SignalRSettings.Backplane>();
            //    if (backplaneSettings is null) throw new InvalidOperationException("Backplane enabled, but no backplane settings in config.");
            //    switch (backplaneSettings.Provider)
            //    {
            //        case "redis":
            //            if (backplaneSettings.StringConnection is null) throw new InvalidOperationException("Redis backplane provider: No connectionString configured.");
            //            services.AddSignalR().AddStackExchangeRedis(backplaneSettings.StringConnection, options =>
            //            {
            //                options.Configuration.AbortOnConnectFail = false;
            //            });
            //            break;

            //        default:
            //            throw new InvalidOperationException($"SignalR backplane Provider {backplaneSettings.Provider} is not supported.");
            //    }

            //    logger.Information($"SignalR Backplane Current Provider: {backplaneSettings.Provider}.");
            }

            return services;
        }

        internal static IEndpointRouteBuilder MapNotifications(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapHub<NotificationHub>("/notifications", options =>
            {
                options.CloseOnAuthenticationExpiration = true;
            });
            return endpoints;
        }
    }
}