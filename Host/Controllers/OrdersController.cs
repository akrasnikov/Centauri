using Host.Entities;
using Host.Interfaces;
using Host.Models;
using Host.Requests;
using Host.Wrappers;
using Microsoft.AspNetCore.Mvc;

namespace Host.Controllers
{
    public class OrdersController : Controller
    {
        private readonly IOrderService _aggregator;

        public OrdersController(IOrderService aggregator)
        {
            _aggregator = aggregator ?? throw new ArgumentNullException(nameof(aggregator));
        }

        [ProducesResponseType(typeof(Response<Order>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Response<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost("order/{id}")]
        public async Task<IActionResult> CreateAsync([FromBody] SearchRequest request)
        {
            var response = await _aggregator.CreateAsync(request, HttpContext.RequestAborted);
            return Ok(response);            
        }


        // TODO добавить pagging
        [ProducesResponseType(typeof(AggregatedDataModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Response<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet("orders/")]
        public async Task<IActionResult> GetAsync([FromQuery] string id)
        {
           var response = await _aggregator.GetAsync(id, HttpContext.RequestAborted);
            return Ok(response);
        }
    }
}
