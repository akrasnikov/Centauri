using PostSharp.Patterns.Diagnostics;
using System.Diagnostics;

namespace Ordering.Host.Infrastructure.Tracing
{
    public static class OrderTracingFactory
    {
        //public static readonly string[] ActivityName = ["host-activity", ActivityNameOrderScope/*, ActivityNameOrderService, ActivityNameIntegrationClient*/];

        public const string ActivityNameOrderSource = "host-activity-order-source";
        public const string ServiceVersion = "1.0";
        public const string ListenerName = "host-listener";
        public const string ServiceName = "order-activity-service";
        public const string CorelationId = "CorrelationId";
        public const string TagCorelationId = "correlation.Id";
        public const string OrderId = "order.id";
        public const string TraceId = "TraceId";
        public const string MethodName = "method-name";
        public const string MethodArgument = "method-argument";

        private static readonly ActivitySource ActivitySource = new(ActivityNameOrderSource);

        public static ActivitySource GetActivitySource()
        {
            return ActivitySource;
        }

        public static Activity StartActivity(string activityName)
        {
            return ActivitySource?.StartActivity(activityName) ?? new(ActivityNameOrderSource);
        }
    }
}
