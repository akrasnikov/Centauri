namespace Host.Integration.Configurations;

internal static class Startup
{
    internal static WebApplicationBuilder AddConfigurations(this WebApplicationBuilder builder)
    {
        const string configurationsDirectory = "Configurations";
        var environmentName = builder.Environment.EnvironmentName.ToLowerInvariant();
        builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environmentName}.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"{configurationsDirectory}/logger.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"{configurationsDirectory}/logger.{environmentName}.json", optional: true, reloadOnChange: true)                
                .AddJsonFile($"{configurationsDirectory}/openapi.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"{configurationsDirectory}/openapi.{environmentName}.json", optional: true, reloadOnChange: true)              
                .AddEnvironmentVariables();
        return builder;
    }
}