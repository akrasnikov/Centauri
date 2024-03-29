using System.Text;

namespace Host.Infrastructure
{
    public sealed class IntegrationFactory
    {
        public HttpRequestMessage Create(string host, HttpMethod method, string? content, string? traceId)
        {
            var request = new HttpRequestMessage();

            var builder = new StringBuilder();
            builder.Append(host);
            var uri = builder.ToString();

            request.RequestUri = new Uri(uri);
            request.Method = method;

            if (!string.IsNullOrWhiteSpace(content))
            {
                request.Content = new StringContent(content, Encoding.UTF8, "application/json");
            }

            if (!string.IsNullOrWhiteSpace(traceId))
            {
                request.Headers.Add("trace-id", traceId);
            }
            return request;
        }
    }
}
