using Microsoft.AspNetCore.Mvc;
using Ordering.Host.Interfaces;
using Ordering.Host.Models;
using Ordering.Host.Requests;
using Ordering.Host.Wrappers;

namespace Ordering.Host.Controllers
{
    public class TicketsController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public TicketsController(IOrderService orderService)
        {
            _orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
        }



        [ProducesResponseType(typeof(Response<OrderModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Response<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost("tickets/searches")]
        public IActionResult Create([FromBody] OrderRequest request)
        {
            var response = _orderService.Create(request, HttpContext.RequestAborted);
            return Ok(response);
        }

        [ProducesResponseType(typeof(Response<OrderModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Response<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPut("tickets/payment")]
        public IActionResult Payment([FromBody] OrderRequest request)
        {
            var response = _orderService.Create(request, HttpContext.RequestAborted);
            return Ok(response);
        }

        [ProducesResponseType(typeof(Response<OrderModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Response<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPut("tickets/commit")]
        public IActionResult Commit([FromBody] OrderRequest request)
        {
            var response = _orderService.Create(request, HttpContext.RequestAborted);
            return Ok(response);
        }

        [ProducesResponseType(typeof(Response<OrderModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Response<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPut("tickets/rollback")]
        public IActionResult Rollback([FromBody] OrderRequest request)
        {
            var response = _orderService.Create(request, HttpContext.RequestAborted);
            return Ok(response);
        }



        // TODO добавить pagging
        [ProducesResponseType(typeof(OrdersModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Response<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet("tickets/check")]
        public async Task<IActionResult> GetAsync([FromQuery] string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException($"\"{nameof(id)}\" не может быть пустым или содержать только пробел.", nameof(id));
            }

            var response = await _orderService.GetAsync(id, HttpContext.RequestAborted);
            return Ok(response);
        }
    }
}
