using PostSharp.Aspects;
using PostSharp.Serialization;
using Serilog;

namespace Ordering.Domain.Tracing.Aspect
{
    [PSerializable]
    public class OrderTracingBoundarysAttribute : OnMethodBoundaryAspect
    {
        public string ActivityName { get; set; } = "default";

        public override void OnEntry(MethodExecutionArgs args)
        {
            using var activity = OrderTracingFactory.StartActivity(ActivityName);
            Log.Logger.Information(ActivityName);
        }

        public override void OnSuccess(MethodExecutionArgs args)
        {
            // Method intentionally left empty.
        }

        public override void OnExit(MethodExecutionArgs args)
        {
            // Method intentionally left empty.
        }

        public override void OnException(MethodExecutionArgs args)
        {
            // Method intentionally left empty.
        }
    }
}
