using Microsoft.Extensions.Primitives;
using Serilog.Context;

namespace Host.Integration
{
    public class RequestContextLoggingMiddleware
    {
        private const string XCorrelationIdHeaderName = "X-Correlation-Id";
        private const string CorrelationIdHeaderName = "custom-correlation-id";
        private readonly RequestDelegate _next;

        public RequestContextLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public Task Invoke(HttpContext context)
        {
            string correlationId = GetCorrelationId(context);

            LogContext.PushProperty("RequestId", correlationId);
            using (LogContext.PushProperty("CorrelationId", correlationId))
            {
                return _next.Invoke(context);
            }
        }

        private static string GetCorrelationId(HttpContext context)
        {
            context.Request.Headers.TryGetValue(
                XCorrelationIdHeaderName, out StringValues xcorrelationId);

            context.Request.Headers.TryGetValue(
                CorrelationIdHeaderName, out StringValues correlationId);

            context.Request.Headers.TryAdd(CorrelationIdHeaderName, correlationId);
            return xcorrelationId.FirstOrDefault() ?? context.TraceIdentifier;
        }
    }
}
