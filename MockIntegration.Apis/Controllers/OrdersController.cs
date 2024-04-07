using Bogus.DataSets;
using Bogus;
using Host.Integration.Models;
using Host.Integration.Wrappers;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Host.Integration.Controllers
{
    public class OrdersController : Controller
    {

        [ProducesResponseType(typeof(IReadOnlyCollection<OrderModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Response<OrderModel>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet("orders/")]
        public async Task<IActionResult> GetAsync([FromQuery] string from, [FromQuery] string to, [FromQuery] DateTime time)
        {
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
