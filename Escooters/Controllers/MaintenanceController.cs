using Microsoft.AspNetCore.Mvc;

namespace Escooters.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Escooters.Models;
    using Microsoft.EntityFrameworkCore;

    public class MaintenanceController : Controller
    {
        private readonly MyDbContext _context;

        public MaintenanceController(MyDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Create()
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            return View();
        }

        [HttpPost]
        public IActionResult Create(Maintenance model)
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            model.UserId = userId.Value;
            model.RequestDate = DateTime.Now;

            _context.Maintenances.Add(model);
            _context.SaveChanges();

            TempData["Success"] = "Maintenance request submitted!";
            return RedirectToAction("Create");
        }


    }

}
