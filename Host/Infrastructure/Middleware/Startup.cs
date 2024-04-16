namespace Host.Infrastructure.Middleware;

public static class Startup
{
    public static IServiceCollection AddExceptionMiddleware(this IServiceCollection services) =>
        services.AddScoped<ExceptionMiddleware>();

    public static IApplicationBuilder UseExceptionMiddleware(this IApplicationBuilder app) =>
        app.UseMiddleware<ExceptionMiddleware>();

    public static IApplicationBuilder UseLoggerMiddleware(this IApplicationBuilder app)
    {
        app.UseMiddleware<RequestContextLoggingMiddleware>();
        return app.UseMiddleware<LogContextMiddleware>();        
    }

}