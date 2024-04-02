using Microsoft.AspNetCore.Mvc;

namespace Host.Controllers
{
    public class MockController : Controller
    {
        // TODO добавить pagging
        //[ProducesResponseType(typeof(IReadOnlyCollection<Order>), StatusCodes.Status200OK)]
        //[ProducesResponseType(typeof(Response<Order>), StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //[HttpGet("orders/")]
        //public async Task<IActionResult> GetAsync([FromQuery] string id)
        //{
        //    _instrumentation.AddOrder();
        //    //var response = await _aggregator.GetFlightsAsync(request, HttpContext.RequestAborted);
        //    return Ok("hello" );
        //}
    }
}
