using Microsoft.AspNetCore.Mvc;
using Ordering.Domain.Models;
using Ordering.Domain.Tracing.Aspect;
using Ordering.Domain.Wrappers;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Banking.Host.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {

        [ProducesResponseType(typeof(IReadOnlyCollection<OrderModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Response<OrderModel>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [TracingInterceptor(ActivityName = "payments/create")]
        [HttpPost("payments/create")]
        public async Task<IActionResult> СreateAsync([FromBody] string id)
        {
             
             
            return Ok( );
        }

        [ProducesResponseType(typeof(IReadOnlyCollection<OrderModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Response<OrderModel>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [TracingInterceptor(ActivityName = "payments/commit")]
        [HttpPut("payments/commit")]
        public async Task<IActionResult> СommitAsync([FromBody] string id)
        {
            

            return Ok();
        }

        [ProducesResponseType(typeof(IReadOnlyCollection<OrderModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Response<OrderModel>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [TracingInterceptor(ActivityName = "payments/rollback")]
        [HttpPut("payments/rollback")]
        public async Task<IActionResult> RollbackAsync([FromBody] string id)
        {
             

            return Ok();
        }

        [ProducesResponseType(typeof(IReadOnlyCollection<OrderModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Response<OrderModel>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [TracingInterceptor(ActivityName = "payments/check")]
        [HttpPut("payments/check")]
        public async Task<IActionResult> CheckAsync([FromBody] string id)
        {


            return Ok();
        }
    }
}
