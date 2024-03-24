using Host.Exceptions;
using System.Net;

namespace Host.Integrations
{
    public class FlightClient : IFlightClient
    {
        private readonly HttpClient _client;
        protected readonly ILogger<FlightClient> _logger;


        public FlightClient(HttpClient client, ILogger<FlightClient> logger)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));

            _client.Timeout = new TimeSpan(0, 10, 0);

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            ServicePointManager.ServerCertificateValidationCallback = (s, c, ch, ssl) => true;

        }

        public async Task<string> SendAsync(HttpRequestMessage requestMessage, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(requestMessage);

            try
            {
                using var response = await _client.SendAsync(requestMessage, cancellationToken);

                var content = await response.Content.ReadAsStringAsync(cancellationToken);

                if (string.IsNullOrWhiteSpace(content))
                {
                    throw new IntegrationException($"flight client - > response is null");
                }

                _logger.LogInformation($"flight client response: {content}");


                if (response.IsSuccessStatusCode)
                {
                    return content;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"flight client errorMessage: {ex.Message}");
                throw;
            }

            throw new IntegrationException($"flight client");

        }
    }
}
