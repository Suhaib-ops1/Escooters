using Microsoft.AspNetCore.Mvc;

namespace Escooters.Controllers
{
    public class ContactMessagesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
