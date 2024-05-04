using FSharp.Json;
using NATS.Client.JetStream;
using NBomber.CSharp;
using NBomber.Http.CSharp;
using Ordering.Domain.Models;
using System.Text.Json;

namespace OrderingTest
{
    class Program
    {
        static void Main(string[] args)
        {

            //var client = new HttpClient();
            //var id = "c6957766-32b5-4662-af0f-8f45b9a29f65";
            //var request = new HttpRequestMessage(HttpMethod.Get, $"http://104.131.189.170/orders?id={id}");
            //var response = client.SendAsync(request).Result;
            //response.EnsureSuccessStatusCode();
            //Console.WriteLine(response.Content.ReadAsStringAsync().Result);

            //Console.ReadLine();

            using var clientStep1 = new HttpClient();
            using var clientStep2 = new HttpClient();

            //var scenario = Scenario.Create("http_scenario", async context =>
            //{
            //    var request =
            //        Http.CreateRequest("POST", url: "http://104.131.189.170:8010/orders/order")
            //        .WithHeader("Accept", "text/html");

            //    var content = new StringContent("{\r\n  \"time\": \"2024-04-19T05:20:13.792Z\",\r\n  \"from\": \"string\",\r\n  \"to\": \"string\"\r\n}", null, "application/json");
            //    request.Content = content;

            //    var response = await Http.Send(httpClient, request);

            //    return response;
            //})
            //.WithoutWarmUp()
            //.WithLoadSimulations(
            //    Simulation.Inject(rate: 100,
            //                      interval: TimeSpan.FromSeconds(1),
            //                      during: TimeSpan.FromSeconds(30))
            //);



            var scenario1 = Scenario.Create("scenario", async context =>
            {
                var step1 = await Step.Run("step_1", context, async () =>
                {

                    var request =
                    Http.CreateRequest("POST", url: "http://104.131.189.170:8010/orders/order")
                    .WithHeader("Accept", "text/html");

                    var content = new StringContent("{\r\n  \"time\": \"2024-04-19T05:20:13.792Z\",\r\n  \"from\": \"string\",\r\n  \"to\": \"string\"\r\n}", null, "application/json");

                    request.Content = content;

                    var response = await clientStep1.SendAsync(request);

                    var id = string.Empty;

                    if (response.IsSuccessStatusCode)
                    {
                        id = await response.Content.ReadAsStringAsync();
                    }
                    else
                    {
                        Response.Fail();
                    }

                    return Response.Ok(payload: id, sizeBytes: 1000);
                });

                var step2 = await Step.Run("step_1", context, async () =>
                {
                    await Task.Delay(1000);


                    if (step1.Payload.IsSome())
                    {

                        var orderModel = JsonSerializer.Deserialize<OrderModel>(step1.Payload.Value);
                        context.Logger.Information(orderModel?.Id);
                        var request = new HttpRequestMessage(HttpMethod.Get, $"http://104.131.189.170/orders?id={orderModel?.Id}");

                        
                        var response = await clientStep2.SendAsync(request);

                        var ordersModel = JsonSerializer.Deserialize<OrderModel>(await response.Content.ReadAsStringAsync());

                        if (response.IsSuccessStatusCode)
                        {

                        }
                        else
                        {
                            Response.Fail();
                        }
                    }
                    return Response.Ok();
                });



                return Response.Ok();
            })
                .WithoutWarmUp()
                .WithLoadSimulations(
                Simulation.Inject(rate: 100, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromSeconds(30))
            );

            NBomberRunner
                .RegisterScenarios(scenario1)
                .Run();
        }
    }
}