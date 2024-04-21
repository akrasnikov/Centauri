using Microsoft.AspNetCore.SignalR.Client;
using System.Collections.Concurrent;
namespace Order.Support
{
    internal class Program
    {
        private static void SetupSignalR(HubConnection connection)
        {
            connection.Reconnecting += error =>
            {
                //Console.WriteLine(connection.State == HubConnectionState.Reconnecting);
                Console.WriteLine("Connection is reconnecting...");
                return Task.CompletedTask;
            };

            connection.Reconnected += connectionId =>
            {
                //Console.WriteLine(connection.State == HubConnectionState.Connected);
                Console.WriteLine("Connection reestablished!");
                return Task.CompletedTask;
            };

            connection.Closed += async (error) =>
            {
                Console.WriteLine("Connection closed. Trying to reconnect...");
                await Task.Delay(new Random().Next(0, 5) * 1000);
                await connection.StartAsync();
            };

            connection.On<string, BasicNotification>("order.progress.notification", (name, message) =>
            {
                _notifications.AddOrUpdate(message.Id, message, (key, value) => message);
                DisplayProgress();
                Console.WriteLine($"Message received from {name}: {message.Progress}");
            });
        }

        //private static ConcurrentBag<BasicNotification> _notifications = [];
        private static ConcurrentDictionary<string, BasicNotification> _notifications = new();
        static void DisplayProgress(/*int completed, int total*/)
        {
            int total = 10;
            Console.Clear();

            foreach (var notification in _notifications)
            {
                int completed = notification.Value.Progress;

                int percentage = (completed * 100) / total;

                Console.WriteLine("Progress:{0} [{1}{2}] {3}% ({4}/{5})",
                    notification.Value.Id,
                    new string('#', percentage / 5),
                    new string(' ', 20 - percentage / 5),
                    percentage,
                    completed,
                    total);
            }

        }
        static async Task Main(string[] args)
        {
            var connection = new HubConnectionBuilder()
            .WithUrl("http://localhost:8010/notifications")
            .WithAutomaticReconnect(new[] { TimeSpan.Zero, TimeSpan.Zero, TimeSpan.FromSeconds(10) })
            .Build();

            SetupSignalR(connection);

            await connection.StartAsync();

            Console.WriteLine("Connection established!");


            List<Task<string>> tasks = new List<Task<string>>();


            for (int i = 0; i < 25; i++)
            {
                tasks.Add(MakeHttpRequestAsync("http://localhost:8010/orders/order"));
            }

            string[] results = await Task.WhenAll(tasks);

            //foreach (string result in results)
            //{
            //    Console.WriteLine(result);
            //}

            Console.ReadLine();
        }

        static async Task<string> MakeHttpRequestAsync(string url)
        {
            using (HttpClient client = new HttpClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Post, url);

                var content = new StringContent("{\r\n  \"time\": \"2024-04-19T05:20:13.792Z\",\r\n  \"from\": \"string\",\r\n  \"to\": \"string\"\r\n}", null, "application/json");
                request.Content = content;
                var response = await client.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    return $"HTTP request to {url} failed with status code: {response.StatusCode}";
                }
            }
        }

        public class BasicNotification
        {
            public string Id { get; set; }
            public int Progress { get; set; }
        }
    }
}
