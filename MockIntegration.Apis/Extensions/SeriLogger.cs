using Serilog;
using Serilog.Sinks.Elasticsearch;

namespace Host.Integration.Extensions
{
    public static class SeriLogger
    {
        public static Action<HostBuilderContext, LoggerConfiguration> Configure =>
            (context, configuration) =>
            {
                configuration
                     .Enrich.FromLogContext()
                    .Enrich.WithMachineName()
                    .WriteTo.Debug()
                    .WriteTo.Console()
                    .WriteTo.Elasticsearch(
                        new ElasticsearchSinkOptions(new Uri("elasticsearch:9200"))
                        {
                            IndexFormat = $"aggregator-applogs-{context.HostingEnvironment.ApplicationName?.ToLowerInvariant().Replace(".", "-")}" +
                                          $"-{context.HostingEnvironment.EnvironmentName?.ToLowerInvariant().Replace(".", "-")}" +
                                          $"-{DateTime.UtcNow:yyyy-MM}",
                            AutoRegisterTemplate = true,
                            NumberOfShards = 2,
                            NumberOfReplicas = 1
                        })
                    .Enrich.WithProperty("Application", context.HostingEnvironment.ApplicationName)
                    .Enrich.WithProperty("ContentRootPath", context.HostingEnvironment.ContentRootPath)
                    .Enrich.WithEnvironmentName()
                    .Enrich.FromLogContext()
                    .ReadFrom.Configuration(context.Configuration);
            };
    } 
}
