using System;
namespace Order.Support
{
    internal class Program
    {
        //static async Task Main(string[] args)
        //{
        //    var client = new HttpClient();
        //    var request = new HttpRequestMessage(HttpMethod.Post, "http://104.131.189.170:8010/orders/order");          
        //    var content = new StringContent("{\r\n  \"time\": \"2024-04-19T05:20:13.792Z\",\r\n  \"from\": \"string\",\r\n  \"to\": \"string\"\r\n}", null, "application/json");
        //    request.Content = content;
        //    var response = await client.SendAsync(request);
        //    response.EnsureSuccessStatusCode();
        //    Console.WriteLine(await response.Content.ReadAsStringAsync());
        //}
        static async Task Main(string[] args)
        {
            Task<string>[] tasks = new Task<string>[]
            {
            MakeHttpRequestAsync("http://104.131.189.170:8010/orders/order"),
            MakeHttpRequestAsync("http://104.131.189.170:8010/orders/order"),
            MakeHttpRequestAsync("http://104.131.189.170:8010/orders/order"),
            MakeHttpRequestAsync("http://104.131.189.170:8010/orders/order"),
            MakeHttpRequestAsync("http://104.131.189.170:8010/orders/order"),
            MakeHttpRequestAsync("http://104.131.189.170:8010/orders/order"),
            MakeHttpRequestAsync("http://104.131.189.170:8010/orders/order"),
            MakeHttpRequestAsync("http://104.131.189.170:8010/orders/order"),
            MakeHttpRequestAsync("http://104.131.189.170:8010/orders/order"),
            MakeHttpRequestAsync("http://104.131.189.170:8010/orders/order"),
            MakeHttpRequestAsync("http://104.131.189.170:8010/orders/order"),
            MakeHttpRequestAsync("http://104.131.189.170:8010/orders/order"),
            MakeHttpRequestAsync("http://104.131.189.170:8010/orders/order"),
            MakeHttpRequestAsync("http://104.131.189.170:8010/orders/order"),
            MakeHttpRequestAsync("http://104.131.189.170:8010/orders/order"),
            MakeHttpRequestAsync("http://104.131.189.170:8010/orders/order"),
            MakeHttpRequestAsync("http://104.131.189.170:8010/orders/order")
            };

            string[] results = await Task.WhenAll(tasks);

            foreach (string result in results)
            {
                Console.WriteLine(result);
            }
        }

        static async Task<string> MakeHttpRequestAsync(string url)
        {
            using (HttpClient client = new HttpClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Post, "http://104.131.189.170:8010/orders/order");
               
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
    }
}
