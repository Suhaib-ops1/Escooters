using Escooters.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Escooters.Controllers
{
    public class AdminController : Controller
    {
        private readonly MyDbContext _context;

        public AdminController(MyDbContext context)
        {
            _context = context;
        }

        public IActionResult Dashboard()
        {
            ViewBag.UserCount = _context.Users.Count();
            ViewBag.BikeCount = _context.Bikes.Count();
            ViewBag.ServiceCount = _context.Services.Count();
            ViewBag.ContactCount = _context.ContactMessages.Count();
            ViewBag.PaymentCount = _context.Payments.Count();

            return View();
        }


        public IActionResult Users()
        {
            var users = _context.Users.ToList();
            return View(users);
        }

        [HttpPost]
        public IActionResult DeleteUser(int id)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserId == id);
            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
            }

            return RedirectToAction("Users");
        }


        public IActionResult ContactMessages()
        {
            var messages = _context.ContactMessages
                .OrderByDescending(m => m.SentDate)
                .ToList();

            return View(messages);
        }

        // View all bikes
        public IActionResult Bikes()
        {
            var bikes = _context.Bikes.ToList();
            return View(bikes);
        }

        // GET: Show Add Bike form
        [HttpGet]
        public IActionResult AddBike()
        {
            return View();
        }
        [HttpGet]
        public IActionResult EditBike(int id)
        {
            var bike = _context.Bikes.FirstOrDefault(b => b.BikeId == id);
            if (bike == null) return NotFound();

            return View(bike);
        }
        [HttpPost]
        public async Task<IActionResult> EditBike(Bike updatedBike, IFormFile ImageFile)
        {
            var bike = _context.Bikes.FirstOrDefault(b => b.BikeId == updatedBike.BikeId);
            if (bike == null) return NotFound();

            bike.Name = updatedBike.Name;
            bike.Type = updatedBike.Type;
            bike.Description = updatedBike.Description;
            bike.PricePerDay = updatedBike.PricePerDay;
            bike.Status = updatedBike.Status;

            if (ImageFile != null && ImageFile.Length > 0)
            {
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Uploads");
                Directory.CreateDirectory(folderPath);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
                var filePath = Path.Combine(folderPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await ImageFile.CopyToAsync(stream);
                }

                bike.ImageUrl = "/Uploads/" + fileName;
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = "Bike updated successfully!";
            return RedirectToAction("ManageBikes");
        }

        // POST: Add new bike
        [HttpPost]
        public async Task<IActionResult> AddBike(Bike bike, IFormFile ImageFile)
        {
            if (ImageFile != null && ImageFile.Length > 0)
            {
                var folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Uploads");
                Directory.CreateDirectory(folder); // Ensure folder exists
                var uniqueName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
                var path = Path.Combine(folder, uniqueName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await ImageFile.CopyToAsync(stream);
                }

                bike.ImageUrl = "/Uploads/" + uniqueName;
            }

            _context.Bikes.Add(bike);
            _context.SaveChanges();

            TempData["Success"] = "Bike added successfully!";
            return RedirectToAction("Bikes");
        }



        [HttpPost]
        public IActionResult UpdateBikeStatus(int id, string status)
        {
            var bike = _context.Bikes.FirstOrDefault(b => b.BikeId == id);
            if (bike != null)
            {
                bike.Status = status;
                _context.SaveChanges();
                TempData["Success"] = "Bike status updated.";
            }

            return RedirectToAction("Bikes");
        }


        public IActionResult Bookings()
        {
            var bookings = _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Bike)
                .OrderByDescending(b => b.PickupDate)
                .ToList();

            return View(bookings);
        }

        public IActionResult Payments(string method, DateTime? fromDate, DateTime? toDate)
        {
            var query = _context.Payments
                .Include(p => p.User)
                .Include(p => p.Booking)
                .AsQueryable();

            if (!string.IsNullOrEmpty(method) && method != "All")
                query = query.Where(p => p.PaymentMethod == method);

            if (fromDate.HasValue)
                query = query.Where(p => p.PaymentDate >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(p => p.PaymentDate <= toDate.Value);

            ViewBag.Method = method;
            ViewBag.From = fromDate?.ToString("yyyy-MM-dd");
            ViewBag.To = toDate?.ToString("yyyy-MM-dd");

            return View(query.OrderByDescending(p => p.PaymentDate).ToList());
        }


        // View all services
        public IActionResult Services()
        {
            var services = _context.Services.ToList();
            return View(services);
        }

        // GET: Add service
        [HttpGet]
        public IActionResult AddService()
        {
            return View();
        }

        // POST: Add service
        [HttpPost]
        public async Task<IActionResult> AddService(Service service, IFormFile IconFile)
        {
            if (IconFile != null && IconFile.Length > 0)
            {
                var folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Uploads");
                Directory.CreateDirectory(folder);
                var uniqueName = Guid.NewGuid().ToString() + Path.GetExtension(IconFile.FileName);
                var filePath = Path.Combine(folder, uniqueName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await IconFile.CopyToAsync(stream);
                }

                service.IconUrl = "/Uploads/" + uniqueName;
            }

            _context.Services.Add(service);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Service added successfully!";
            return RedirectToAction("Services");
        }

        // GET: Edit service
        [HttpGet]
        public IActionResult EditService(int id)
        {
            var service = _context.Services.Find(id);
            if (service == null) return NotFound();

            return View(service);
        }

        // POST: Edit service
        [HttpPost]
        public async Task<IActionResult> EditService(Service updated, IFormFile IconFile)
        {
            var service = _context.Services.Find(updated.ServiceId);
            if (service == null) return NotFound();

            service.Title = updated.Title;
            service.Description = updated.Description;

            if (IconFile != null && IconFile.Length > 0)
            {
                var folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Uploads");
                var uniqueName = Guid.NewGuid().ToString() + Path.GetExtension(IconFile.FileName);
                var filePath = Path.Combine(folder, uniqueName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await IconFile.CopyToAsync(stream);
                }

                service.IconUrl = "/Uploads/" + uniqueName;
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = "Service updated!";
            return RedirectToAction("Services");
        }

        [HttpPost]
        public IActionResult DeleteService(int id)
        {
            var service = _context.Services.Find(id);
            if (service == null)
                return NotFound();

            _context.Services.Remove(service);
            _context.SaveChanges();

            TempData["Success"] = "Service deleted successfully.";
            return RedirectToAction("Services");
        }


        public IActionResult MaintenanceRequests(string userName, string bikeName)
        {
            var requests = _context.Maintenances
                .Include(m => m.User)
                .AsQueryable();

            if (!string.IsNullOrEmpty(userName))
                requests = requests.Where(r => r.User.FullName.Contains(userName));

            if (!string.IsNullOrEmpty(bikeName))
                requests = requests.Where(r => r.BikeName.Contains(bikeName));

            ViewBag.UserName = userName;
            ViewBag.BikeName = bikeName;

            return View(requests.OrderByDescending(r => r.RequestDate).ToList());
        }


        [HttpPost]
        public IActionResult DeleteMaintenance(int id)
        {
            var record = _context.Maintenances.Find(id);
            if (record == null)
                return NotFound();

            _context.Maintenances.Remove(record);
            _context.SaveChanges();

            TempData["Success"] = "Maintenance request deleted.";
            return RedirectToAction("MaintenanceRequests");
        }



    }






}
