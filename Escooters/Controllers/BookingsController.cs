﻿using Microsoft.AspNetCore.Mvc;

namespace Escooters.Controllers
{
    public class BookingsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
