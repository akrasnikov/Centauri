using Microsoft.AspNetCore.Mvc;

namespace Host.Controllers
{
    public class OrdersController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
