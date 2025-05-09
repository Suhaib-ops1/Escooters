using Microsoft.AspNetCore.Mvc;

namespace Escooters.Controllers
{
    public class PaymentsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
