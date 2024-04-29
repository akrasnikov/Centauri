using PostSharp.Aspects;
using PostSharp.Serialization;
using Serilog;
using System.Diagnostics;
using System.Text;
using System.Text.Json;


namespace Ordering.Infrastructure.Infrastructure.Logging.PostSharp
{
    [PSerializable]
    public class CustomLogAttribute : OnMethodBoundaryAspect
    {
        /// <summary>
        /// On Method Entry
        /// </summary>
        /// <param name="args"></param>
        public override void OnEntry(MethodExecutionArgs args)
        {
            string className = "defaultClass", methodName = "defaultMethod";

            if (args.Method != null && args.Method.DeclaringType != null)
            {
                className = $"{args.Method.DeclaringType.Namespace}.{args.Method.DeclaringType.Name}";
            }
            if (args.Method != null) methodName = args.Method.Name.ToLowerInvariant();

            string parameters = string.Empty;

            if (args.Method.IsGenericMethod && args.Arguments != null && args.Arguments.Count > 0)
            {
                var collection = args.Method.GetParameters().ToDictionary(key => key.Name, value => args?.Arguments[value.Position]);

                parameters += $" args: {JsonSerializer.Serialize(collection)}";
            }
            Log.Logger.Information($"className: {className}; methodName:{methodName};arguments:{parameters}");
        }

        /// <summary>
        /// On Method success
        /// </summary>
        /// <param name="args"></param>
        public override void OnSuccess(MethodExecutionArgs args)
        {
            Log.Logger.Information($"OnSuccess : {(args.Method != null ? args.Method.Name : "")}");
            var returnValue = args.ReturnValue;
            Log.Logger.Information($"ReturnValue : {returnValue}");
        }

        /// <summary>
        /// On Method Exception
        /// </summary>
        /// <param name="args"></param>
        public override void OnException(MethodExecutionArgs args)
        {
            if (args.Exception != null)
                Log.Logger.Information($"OnException : {(!string.IsNullOrEmpty(args.Exception.Message) ? args.Exception.Message : "")}");

            var Message = args.Exception.Message;
            var StackTrace = args.Exception.StackTrace;

            Log.Logger.Information($"Application has got exception in method-{args.Method.Name} and message is {Message}");

            // or you can send email notification
        }

        /// <summary>
        /// On Method Exit
        /// </summary>
        /// <param name="args"></param>
        public override void OnExit(MethodExecutionArgs args)
        {
        }
    }
}
