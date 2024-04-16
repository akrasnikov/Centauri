using Bogus;
using Host.Integration.Models;
using Host.Integration.Services;
using Host.Integration.Wrappers;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Reflection;

namespace Host.Integration.Controllers
{
    public class OrdersController : Controller
    {
        private  readonly IDummyClass _dummyClass;
        private readonly ILogger<OrdersController> _logger;



        public OrdersController(IDummyClass dummyClass, ILogger<OrdersController> logger)
        {
            _dummyClass = dummyClass ?? throw new ArgumentNullException(nameof(dummyClass));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [ProducesResponseType(typeof(IReadOnlyCollection<OrderModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Response<OrderModel>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet("orders/")]
        public async Task<IActionResult> GetAsync([FromQuery] string from, [FromQuery] string to, [FromQuery] DateTime time)
        {
            _logger.LogInformation("get orders integration");
            using var activitySource = ActivityProvider.Create();
            using var activity = activitySource.StartActivity($"{ActivityProvider.MethodName}: get orders ");
            activity?.SetTag(ActivityProvider.MethodArgument, $"from:{from} - to: {to} - time: {time}");

            var dummy = _dummyClass.GetDummy("hello");
            var activityId = Activity.Current.Id;
            var id = HttpContext.TraceIdentifier;
            var orderGenerator = GetOrderGenerator(from, to, time);
            var NumberOfOrders = 2;
            var generatedOrders = orderGenerator.Generate(NumberOfOrders);
            List<OrderModel> Orders = new();
            Orders.AddRange(generatedOrders);

            return Ok(generatedOrders);
        }

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
