using System.Diagnostics;

namespace Host.Integration.Services
{
    public static class ActivityProvider
    {
        public const string ServiceVersion = "1.0";
        public const string ActivityName = "host-integration-activity";
        public const string ListenerName = "host-integration-listener";
        public const string ServiceName = "host-integration-service";
        public const string MethodName = "method-name";
        public const string MethodArgument = "method-argument";

        //public static readonly ActivitySource Instance = new(ActivityName);
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
