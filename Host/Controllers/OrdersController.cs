using Host.Extensions;
using Host.Infrastructure.Logging.PostSharp;
using Host.Interfaces;
using Host.Models;
using Host.Requests;
using Host.Wrappers;
using Microsoft.AspNetCore.Mvc;

namespace Host.Controllers
{

    [LogTrace]
    public class OrdersController : Controller
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
        }



        [ProducesResponseType(typeof(Response<OrdersModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Response<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost("orders/order")]
        public async Task<IActionResult> CreateAsync([FromBody] OrderRequest request)
        {
            var response = await _orderService.CreateAsync(request, HttpContext.RequestAborted);
            return Ok(response);
        }


        // TODO добавить pagging
        [ProducesResponseType(typeof(OrdersModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Response<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet("orders")]
        public async Task<IActionResult> GetAsync([FromQuery] string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException($"\"{nameof(id)}\" не может быть пустым или содержать только пробел.", nameof(id));
            }

            var response = await  _orderService.GetAsync(id, HttpContext.RequestAborted);
            return Ok(response);
        }
    }
}
