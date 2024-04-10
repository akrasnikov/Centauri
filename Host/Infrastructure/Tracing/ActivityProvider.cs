using System.Diagnostics;

namespace Host.Infrastructure.Tracing
{
    public static class ActivityProvider
    {
        public const string ServiceVersion = "1.0";
        public const string ActivityName = "host-activity";
        public const string ListenerName = "host-listener";
        public const string ServiceName = "host-service";
        public const string MethodName = "method-name";
        public const string MethodArgument = "method-argument";

        public static ActivitySource Create(string? activityName = null)
        {
            if (string.IsNullOrWhiteSpace(activityName))
            {
                return new(ActivityName);
            }

            return new(activityName);
        }
    }
}
