using Host.Entities;
using Host.Interfaces;
using Host.Metrics;
using Host.Requests;
using Host.Wrappers;
using Microsoft.AspNetCore.Mvc;

namespace Host.Controllers
{
    public class OrdersController : Controller
    {
        private readonly IOrderService _aggregator;
        private readonly OrderInstrumentation _instrumentation;

        public OrdersController(IOrderService aggregator, OrderInstrumentation instrumentation)
        {
            _aggregator = aggregator ?? throw new ArgumentNullException(nameof(aggregator));
            _instrumentation = instrumentation ?? throw new ArgumentNullException(nameof(instrumentation));
        }

        [ProducesResponseType(typeof(Response<Order>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Response<Order>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost("order/{id}")]
        public async Task<IActionResult> CreateAsync([FromBody] SearchRequest request)
        {
            var response = await _aggregator.CreateAsync(request, HttpContext.RequestAborted);
            return Ok(response);            
        }


        // TODO добавить pagging
        [ProducesResponseType(typeof(IReadOnlyCollection<Order>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Response<Order>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet("orders/")]
        public async Task<IActionResult> GetAsync([FromQuery] string id)
        {
            _instrumentation.AddOrder();
            //var response = await _aggregator.GetFlightsAsync(request, HttpContext.RequestAborted);
            return Ok("hello" );
        }
    }
}
