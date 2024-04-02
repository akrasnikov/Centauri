using Host.Logs;

namespace Host.Services
{
   
    public interface IDummyService
    {
        string Message(string arg);
    }

    [Log]
    public class DummyService:IDummyService
    {
        public string Message(string arg)
        {
            return arg + "change";
        }
    }
}
