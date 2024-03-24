
namespace Host.Integrations
{
    public interface IFlightClient
    {
        Task<string> SendAsync(HttpRequestMessage requestMessage, CancellationToken cancellationToken = default);
    }
}