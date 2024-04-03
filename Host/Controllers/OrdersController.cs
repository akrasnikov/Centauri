using Host.Logs;
using Host.Models;
using Host.Services;
using Host.Wrappers;
using Microsoft.AspNetCore.Mvc;

namespace Host.Controllers
{
    [CustomLog]
    public class OrdersController : Controller
    {
        private readonly IDummyService _orderService;

        public OrdersController(IDummyService orderService)
        {
            _orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
        }

        //[ProducesResponseType(typeof(Response<Order>), StatusCodes.Status200OK)]
        //[ProducesResponseType(typeof(Response<string>), StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //[HttpPost("order/{id}")]
        //public async Task<IActionResult> CreateAsync([FromBody] OrderRequest request)
        //{
        //    var response = await _orderService.CreateAsync(request, HttpContext.RequestAborted);
        //    return Ok(response);            
        //}


        // TODO добавить pagging
        [ProducesResponseType(typeof(OrdersModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Response<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet("orders/")]
        public async Task<IActionResult> GetAsync([FromQuery] string id)
        {
           var response =  _orderService.Message("hello");
            return Ok(response);
        }
    }
}
