namespace Host.Options
{
#nullable disable
    public class RequestInfo
    {
        public string Url { get; set; }
        public string Method { get; set; }
        public Dictionary<string, string> QueryParams { get; set; }
    }
}
