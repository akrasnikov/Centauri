using Microsoft.AspNetCore.Mvc;

namespace Host.Controllers
{
    public class CarsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
