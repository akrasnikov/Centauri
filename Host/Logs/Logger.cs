using Castle.DynamicProxy;
using System.Diagnostics;

namespace Host.Logs
{
    public class DummyLogger : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            Debug.WriteLine($"Calling {invocation.Method.Name} from Proxy");
            invocation.Proceed();
        }
    }
}
