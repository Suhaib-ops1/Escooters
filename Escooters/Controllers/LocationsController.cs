using Microsoft.AspNetCore.Mvc;

namespace Escooters.Controllers
{
    public class LocationsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
