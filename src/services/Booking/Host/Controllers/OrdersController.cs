using Bogus;
using Booking.Host.Models;
using Booking.Host.Tracing.Aspect;
using Booking.Host.Wrappers;
using Microsoft.AspNetCore.Mvc;

namespace Booking.Host.Controllers
{
    public class OrdersController : Controller
    {
        private readonly ILogger<OrdersController> _logger;
        public OrdersController(ILogger<OrdersController> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [ProducesResponseType(typeof(IReadOnlyCollection<OrderModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Response<OrderModel>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [OrderTracingInterceptor(ActivityName = "get-mock-order")]
        [HttpGet("orders/")]
        public async Task<IActionResult> GetAsync([FromQuery] string from, [FromQuery] string to, [FromQuery] DateTime time)
        {
            _logger.LogInformation("get orders integration");
            await Task.Delay(TimeSpan.FromMilliseconds(new Random().Next(0, 10) * 100));
            var generatedOrders = GetOrderGenerator(from, to, time).Generate(2);
            return Ok(generatedOrders);
        }


        [OrderTracingInterceptor(ActivityName = "mock-generator-order")]
        private static Faker<OrderModel> GetOrderGenerator(string @from, string to, DateTime time)
        {
            if (time == default)
            {
                time = DateTime.Now;
            }

            return new Faker<OrderModel>()
                .RuleFor(e => e.Id, _ => Guid.NewGuid())
                .RuleFor(e => e.To, f => @from ?? f.Address.City())
                .RuleFor(e => e.From, f => to ?? f.Address.City())
                .RuleFor(e => e.Time, f => f.Date.BetweenOffset(time, DateTimeOffset.UtcNow.AddDays(10)));
        }
    }
}
