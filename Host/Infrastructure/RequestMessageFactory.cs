using Host.Infrastructure.HttpClients;
using Microsoft.Extensions.Options;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.Json;

namespace Host.Infrastructure
{
    [Experimental("request_message_factory")]
    public sealed class RequestMessageFactory
    {
        public static HttpRequestMessage Create<T>(
            HttpMethod method, 
            Dictionary<string, string>? queryParams, 
            T content, 
            string? traceId,
            IOptions<IntegrationOptions> options) where T : class
        {
            using var request = new HttpRequestMessage();

            var uriBuilder = new UriBuilder("");

            if (queryParams != null)
            {
                uriBuilder.Query = string.Join("&", queryParams.Select(kvp => $"{kvp.Key}={kvp.Value}"));
            }

            request.RequestUri = uriBuilder.Uri;

            request.Method = method;

            if (content != null)
            {
                request.Content = new StringContent(JsonSerializer.Serialize(content), Encoding.UTF8, "application/json");
            }

            if (!string.IsNullOrWhiteSpace(traceId))
            {
                request.Headers.Add("trace-id", traceId);
            }

            return request;
        }

        public static HttpRequestMessage Create(
            HttpMethod method,
            Dictionary<string, string>? queryParams,            
            string? traceId,
            IOptions<IntegrationOptions> options)
        {
            using var request = new HttpRequestMessage();

            var uriBuilder = new UriBuilder("options.Value.Url");

            if (queryParams != null)
            {
                uriBuilder.Query = string.Join("&", queryParams.Select(kvp => $"{kvp.Key}={kvp.Value}"));
            }

            request.RequestUri = uriBuilder.Uri;

            request.Method = method;

            if (!string.IsNullOrWhiteSpace(traceId))
            {
                request.Headers.Add("trace-id", traceId);
            }

            return request;
        }
    }
}
