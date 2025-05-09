using Microsoft.AspNetCore.Mvc;

namespace Escooters.Controllers
{
    public class UsersController : Controller
    {
        public IActionResult Home()
        {
            return View();
        }
        
        public IActionResult About()
        {
            return View();
        }
        public IActionResult Service() {
            return View();
        }
        public IActionResult Bike()
        {
            return View();
        }
        public IActionResult Bike_detailes()
        {
            return View();
        }
        public IActionResult Contact()
        {
            return View();
        }
    }
}
