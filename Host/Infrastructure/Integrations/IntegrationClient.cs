using Host.Exceptions;
using System.Text.Json;

namespace Host.Infrastructure.Integrations
{
    public class IntegrationClient<T> where T : class
    {
        private readonly HttpClient _client;
        private readonly ILogger<T> _logger;

        public IntegrationClient(HttpClient client, ILogger<T> logger)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _client.Timeout = new TimeSpan(0, 10, 0);
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        public async Task<T> SendAsync(HttpRequestMessage requestMessage, CancellationToken cancellationToken = default) where T : class
        {
            ArgumentNullException.ThrowIfNull(requestMessage);
            try
            {
                using var response = await _client.SendAsync(requestMessage, cancellationToken);

                var content = await response.Content.ReadAsStringAsync(cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    if (string.IsNullOrWhiteSpace(content))
                    {
                        throw new InfrastructureException($"integration client - > response is null");
                    }
                    _logger.LogInformation($"integration client response: {content}");

                    return JsonSerializer.Deserialize<T>(content);
                }

                throw new InfrastructureException($"client");
            }
            catch (Exception)
            {
                _logger.LogError($"integration client error");
                throw;
            }            
        }
    }
}
