using PostSharp.Aspects;
using PostSharp.Serialization;
using Serilog;
using Serilog.Context;

namespace Host.Integration.Tracing.Aspect
{
    [PSerializable]
    public class OrderTracingInterceptorAttribute : MethodInterceptionAspect
    {
        public string ActivityName { get; set; } = "default";
        public override void OnInvoke(MethodInterceptionArgs args)
        {
            //var method = args.Method;
            //var parameters = method.GetParameters();
            //// Accessing method parameters
            //for (int i = 0; i < parameters.Length; i++)
            //{
            //    var parameter = parameters[i];
            //    var parameterName = parameter.Name;
            //    var parameterType = parameter.ParameterType;
            //    var parameterValue = args.Arguments[i];
            //    Debug.WriteLine($"Parameter Name: {parameterName}, Type: {parameterType}, Value: {parameterValue}");
            //}


            Log.Logger.Information(ActivityName);

            using var activity = OrderTracingFactory.StartActivity(ActivityName);
            using var log = LogContext.PushProperty(OrderTracingFactory.TraceId, activity?.TraceId);
            {
                args.Proceed();
            }
        }
    }
}
