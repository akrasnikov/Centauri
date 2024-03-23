using Microsoft.AspNetCore.Mvc;

namespace Host.Controllers
{
    public class CruisesController : ControllerBase
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
