namespace Host.Infrastructure.HttpClients
{
#nullable disable
    public class IntegrationOptions
    {
        public string Url { get; set; }
        public List<RequestInfo> Requests { get; set; }
    }
}
