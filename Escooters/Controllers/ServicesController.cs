﻿using Microsoft.AspNetCore.Mvc;

namespace Escooters.Controllers
{
    public class ServicesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
