using Host.Extensions;
using Host.Models;
using Host.Wrappers;
using Microsoft.AspNetCore.Mvc;

namespace Host.Controllers
{

    public class OrdersController : Controller
    {
        private readonly IDummyService _dummyService;
        public OrdersController(IDummyService dummyService)
        {
            _dummyService = dummyService;
        }

        //[ProducesResponseType(typeof(Response<Order>), StatusCodes.Status200OK)]
        //[ProducesResponseType(typeof(Response<string>), StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //[HttpPost("orders/order")]
        //public async Task<IActionResult> CreateAsync([FromBody] OrderRequest request)
        //{
        //    var response = await _orderService.CreateAsync(request, HttpContext.RequestAborted);
        //    return Ok(response);            
        //}


        // TODO добавить pagging
        [ProducesResponseType(typeof(OrdersModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Response<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet("orders/{id}")]
        public async Task<IActionResult> GetAsync([FromQuery] string id)
        {
           var response = _dummyService.Message("hello");
            return Ok();
        }
    }
}
