namespace Host.Infrastructure.Integrations
{
    public interface IIntegrationClient
    {
        Task<string> SendAsync(HttpRequestMessage requestMessage, CancellationToken cancellationToken = default);
    }
}