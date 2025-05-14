using Escooters.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Escooters.Controllers
{
    public class ContactController : Controller
    {
        private readonly MyDbContext _context;

        public ContactController(MyDbContext context)
        {
            _context = context;
        }
        [HttpPost]
        public IActionResult Contact(string FullName, string Email, string PhoneNumber, string Subject, string Message)
        {
            var contactMessage = new ContactMessage
            {
                FullName = FullName,
                Email = Email,
                PhoneNumber = PhoneNumber,
                Subject = Subject,
                Message = Message,
                SentDate = DateTime.Now
            };

            _context.ContactMessages.Add(contactMessage);
            _context.SaveChanges();

            TempData["Success"] = "Your message has been sent!";
            return View();
        }

    }
}
