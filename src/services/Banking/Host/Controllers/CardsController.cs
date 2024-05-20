using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Ordering.Domain.Models;
using Ordering.Domain.Tracing.Aspect;
using Ordering.Domain.Wrappers;

namespace Banking.Host.Controllers
{
    public class CardsController : ControllerBase
    {
        [ProducesResponseType(typeof(IReadOnlyCollection<OrderModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Response<OrderModel>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [TracingInterceptor(ActivityName = "cards/balance")]
        [HttpGet("cards/balance")]
        public async Task<IActionResult> СreateAsync([FromQuery] string id)
        {


            return Ok();
        }

    }
}
