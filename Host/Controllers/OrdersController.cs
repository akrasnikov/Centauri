using Host.Entities;
using Host.Interfaces;
using Host.Requests;
using Host.Wrappers;
using Microsoft.AspNetCore.Mvc;

namespace Host.Controllers
{
    public class OrdersController : Controller
    {
        private readonly IOrderAggregator _aggregator;

        public OrdersController(IOrderAggregator aggregator)
        {
            _aggregator = aggregator ?? throw new ArgumentNullException(nameof(aggregator));
        }

        [ProducesResponseType(typeof(Response<Order>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Response<Order>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost("travels/order/{id}")]
        public async Task<IActionResult> CreateAsync([FromBody] SearchRequest request)
        {

            var response = await _aggregator.GetAsync(request, HttpContext.RequestAborted);
            return Ok(response);
            
        }


        // TODO добавить pagging
        [ProducesResponseType(typeof(IReadOnlyCollection<Order>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Response<Order>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet("travels/orders/{id:guid}")]
        public async Task<IActionResult> GetAsync([FromQuery] Guid id)
        {

            var response = await _aggregator.GetFlightsAsync(request, HttpContext.RequestAborted);
            return Ok(response);
        }
    }
}
