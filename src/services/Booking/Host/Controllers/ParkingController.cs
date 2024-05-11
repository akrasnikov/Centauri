using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Booking.Host.Controllers
{
    public class ParkingController : Controller
    {
        // GET: ParkingController
        public ActionResult Index()
        {
            return View();
        }

        // GET: ParkingController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ParkingController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ParkingController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ParkingController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ParkingController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ParkingController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ParkingController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
