using PostSharp.Aspects;
using PostSharp.Serialization;
using System.Drawing;

namespace Booking.Host.PostSharp
{
    [PSerializable]

    public class LoggingAspect : OnMethodBoundaryAspect
    {
        public override void OnEntry(MethodExecutionArgs args)
        {
            var color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"enter {args.Method.Name} with param: {string.Join(", ", args.Arguments)}");
            Console.ForegroundColor = color;
        }

        public override void OnExit(MethodExecutionArgs args)
        {
            var color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"exit from method {args.Method.Name} return value: {args.ReturnValue}");
            Console.ForegroundColor = color;
        }
    }
}
