namespace Host.Integration.Services
{
    public interface IDummyClass
    {
        string GetDummy(string text);
    }

    public class DummyClass : IDummyClass
    {
        public string GetDummy(string text)
        {
            using var activitySource = ActivityProvider.Create();
            using var activity = activitySource.StartActivity($"{ActivityProvider.MethodName}: {nameof(GetDummy).ToLowerInvariant()}");
            activity?.SetTag(ActivityProvider.MethodArgument, text);
            return $"dummy - {text}";
        }
    }
}
