using PostSharp.Patterns.Diagnostics;
using System.Diagnostics;

namespace Host.Integration.Tracing
{
    public static class OrderTracingFactory
    {
        //public static readonly string[] ActivityName = ["host-activity", ActivityNameOrderSource/*, ActivityNameOrderService, ActivityNameIntegrationClient*/];

        public const string ActivityNameOrderSource = "mock-activity-order-source";
        public const string ServiceVersion = "1.0";          
        public const string CorelationId = "CorrelationId";        
        public const string OrderId = "order.id";
        public const string TraceId = "TraceId";
        public const string ListenerName = "host-integration-listener";
        public const string ServiceName = "mock-activity-order-service";
        public const string MethodName = "method-name";
        public const string MethodArgument = "method-argument";
        public const string TagCorelationId = "corelation.Id";

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
