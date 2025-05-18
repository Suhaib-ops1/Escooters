using Escooters.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rotativa.AspNetCore;

namespace Escooters.Controllers
{
    public class UsersController : Controller
    {
        private readonly MyDbContext _context;

        public UsersController(MyDbContext context)
        {
            _context = context;
        }
        public IActionResult Home()
        {
            
                return View();
            
            
        }
        
        public IActionResult About()
        {
            return View();
        }
        public IActionResult Service() {
            var services = _context.Services.ToList();
            return View(services);
        }
        public IActionResult Bike(string searchName, string searchType, decimal? minPrice, decimal? maxPrice)
        {
            var query = _context.Bikes
                .Where(b => b.Status == "Available")
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchName))
                query = query.Where(b => b.Name.Contains(searchName));

            if (!string.IsNullOrEmpty(searchType))
                query = query.Where(b => b.Type.Contains(searchType));

            if (minPrice.HasValue)
                query = query.Where(b => b.PricePerDay >= minPrice);

            if (maxPrice.HasValue)
                query = query.Where(b => b.PricePerDay <= maxPrice);

            ViewBag.Name = searchName;
            ViewBag.Type = searchType;
            ViewBag.Min = minPrice;
            ViewBag.Max = maxPrice;

            return View(query.ToList());
        }


        public IActionResult RentBike(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var bike = _context.Bikes.FirstOrDefault(b => b.BikeId == id && b.Status == "Available");
            if (bike == null)
                return NotFound();

            return View("BookingForm", bike);
        }

        [HttpPost]
        public IActionResult ConfirmBooking(int BikeId, DateTime PickupDate, DateTime ReturnDate)
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null)
            {
                TempData["Error"] = "Please log in to complete booking.";
                return RedirectToAction("Login", "Account");
            }

            var bike = _context.Bikes.FirstOrDefault(b => b.BikeId == BikeId);
            if (bike == null || bike.Status != "Available")
            {
                TempData["Error"] = "Bike not available.";
                return RedirectToAction("Bike");
            }

            int totalDays = (ReturnDate - PickupDate).Days;
            if (totalDays <= 0)
            {
                TempData["Error"] = "Return date must be after pickup date.";
                return RedirectToAction("RentBike", new { id = BikeId });
            }

            var booking = new Booking
            {
                UserId = userId.Value,
                BikeId = BikeId,
                PickupDate = PickupDate,
                ReturnDate = ReturnDate,
                PickupLocationId = 1, // still required for DB schema
                DropoffLocationId = 1, // still required for DB schema
                TotalCost = totalDays * bike.PricePerDay,
                Status = "Confirmed"
            };

            _context.Bookings.Add(booking);

            bike.Status = "Rented";

            _context.SaveChanges();

            TempData["BookingConfirmed"] = booking.BookingId;
            TempData["Success"] = "Booking confirmed successfully. Proceed to payment.";

            //return RedirectToAction("Payment", "Users", new { id = BikeId });
            return RedirectToAction("Payment", "Users", new { bookingId = booking.BookingId });


        }

        public IActionResult Payment(int bookingId)
        {
            var booking = _context.Bookings
                .Include(b => b.Bike)
                .Include(b => b.User)
                .FirstOrDefault(b => b.BookingId == bookingId);

            if (booking == null)
                return NotFound();

            return View(booking);
        }

        [HttpPost]
        public IActionResult ProcessPayment(int BookingId, decimal Amount, string PaymentMethod)
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var booking = _context.Bookings.Include(b => b.Bike).FirstOrDefault(b => b.BookingId == BookingId);
            if (booking == null)
                return NotFound();

            var payment = new Payment
            {
                BookingId = BookingId,
                UserId = userId.Value,
                Amount = Amount,
                PaymentMethod = PaymentMethod,
                PaymentDate = DateTime.Now,
                Status = "Paid"
            };

            _context.Payments.Add(payment);
            booking.Status = "Confirmed"; // Optional: Update booking status
            _context.SaveChanges();

            TempData["Success"] = "Payment successful! Thank you.";
            return RedirectToAction("MyBookings");
        }


        public IActionResult MyBookings()
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var bookings = _context.Bookings
                .Include(b => b.Bike)
                .Include(b => b.Payments)
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.PickupDate)
                .ToList();

            return View(bookings);
        }

        [HttpPost]
        public IActionResult CancelBooking(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var booking = _context.Bookings.FirstOrDefault(b => b.BookingId == id && b.UserId == userId);

            if (booking == null || booking.Payments.Any())
            {
                TempData["Error"] = "Cannot cancel this booking.";
                return RedirectToAction("MyBookings");
            }

            // Set bike back to available
            var bike = _context.Bikes.FirstOrDefault(b => b.BikeId == booking.BikeId);
            if (bike != null)
            {
                bike.Status = "Available";
            }

            _context.Bookings.Remove(booking);
            _context.SaveChanges();

            TempData["Success"] = "Booking cancelled successfully.";
            return RedirectToAction("MyBookings");
        }


        public IActionResult DownloadInvoice(int bookingId)
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var booking = _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Bike)
                .Include(b => b.Payments)
                .FirstOrDefault(b => b.BookingId == bookingId && b.UserId == userId);

            if (booking == null || !booking.Payments.Any())
            {
                TempData["Error"] = "Invalid invoice request.";
                return RedirectToAction("MyBookings");
            }

            return new ViewAsPdf("Invoice", booking)
            {
                FileName = $"Invoice_Booking_{booking.BookingId}.pdf"
            };
        }


        public IActionResult MyMaintenance()
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var list = _context.Maintenances
                .Where(m => m.UserId == userId)
                .OrderByDescending(m => m.RequestDate)
                .ToList();

            return View(list);
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
