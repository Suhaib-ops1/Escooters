using Microsoft.AspNetCore.Mvc;

namespace Escooters.Controllers
{
    public class BikesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
