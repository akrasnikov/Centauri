using Host.PostSharp;

namespace Host.Extensions
{
    [CustomLoggingFormatter]
    public class DummyService : IDummyService
    {
        public string Message(string v)
        {
            return "hello";
        }
    }
}