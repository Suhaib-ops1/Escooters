using Escooters.Models;
using Microsoft.AspNetCore.Mvc;

public class AccountController : Controller
{
    private readonly MyDbContext _context;

    public AccountController(MyDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

   [HttpPost]
public IActionResult Register(string FullName, string Email, string Password, string ConfirmPassword)
{
    if (Password != ConfirmPassword)
    {
        ViewBag.Error = "Passwords do not match.";
        return View();
    }

    var existingUser = _context.Users.FirstOrDefault(u => u.Email == Email);
    if (existingUser != null)
    {
        ViewBag.Error = "Email is already registered.";
        return View();
    }

    var user = new User
    {
        FullName = FullName,
        Email = Email,
        PasswordHash = Password,
        RegistrationDate = DateTime.Now
    };

    _context.Users.Add(user);
    _context.SaveChanges();

    TempData["Success"] = "Registration successful. You can now log in.";
    return RedirectToAction("Login");
}


    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Login(string Email, string Password)
    {
        var user = _context.Users.FirstOrDefault(u => u.Email == Email && u.PasswordHash == Password);
        if (user != null)
        {
            HttpContext.Session.SetInt32("UserID", user.UserId);
            HttpContext.Session.SetString("UserName", user.FullName);

            // Simple check for admin
            if (Email == "Admin@gmail.com"&& Password=="Admin") // Change to your admin email
            {
                return RedirectToAction("Dashboard", "Admin");
            }

            return RedirectToAction("Home", "Users");
        }

        ViewBag.Error = "Invalid email or password.";
        return View();
    }


    [HttpGet]
    public IActionResult Profile()
    {
        int? userId = HttpContext.Session.GetInt32("UserID");
        if (userId == null) return RedirectToAction("Login");

        var user = _context.Users.Find(userId);
        return View(user);
    }



    [HttpGet]
    public IActionResult EditProfile()
    {
        int? userId = HttpContext.Session.GetInt32("UserID");
        if (userId == null) return RedirectToAction("Login");

        var user = _context.Users.Find(userId);
        return View(user);
    }

    [HttpPost]
    public async Task<IActionResult> EditProfile(User updatedUser, IFormFile ImageFile)
    {
        var user = _context.Users.FirstOrDefault(u => u.UserId == updatedUser.UserId);
        if (user == null)
            return NotFound();

        // Email check
        var emailExists = _context.Users.Any(u => u.Email == updatedUser.Email && u.UserId != updatedUser.UserId);
        if (emailExists)
        {
            TempData["Error"] = "This email is already taken.";
            return RedirectToAction("Profile");
        }

        user.FullName = updatedUser.FullName;
        user.PhoneNumber = updatedUser.PhoneNumber;
        user.Email = updatedUser.Email;

        if (ImageFile != null && ImageFile.Length > 0)
        {
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Uploads");
            Directory.CreateDirectory(uploadsFolder); // Make sure folder exists

            var uniqueFileName = Guid.NewGuid().ToString() + ImageFile.FileName;
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await ImageFile.CopyToAsync(stream);
            }

            user.ProfileImage = "/Uploads/" + uniqueFileName; // This is the final image path
        }

        await _context.SaveChangesAsync();
        TempData["Success"] = "Profile updated!";
        return RedirectToAction("Profile");
    }

    [HttpGet]
    public IActionResult ResetPassword()
    {
        int? userId = HttpContext.Session.GetInt32("UserID");
        if (userId == null) return RedirectToAction("Login");

        return View();
    }

    [HttpPost]
    public IActionResult ResetPassword(string currentPassword, string newPassword, string confirmPassword)
    {
        int? userId = HttpContext.Session.GetInt32("UserID");
        if (userId == null) return RedirectToAction("Login");

        var user = _context.Users.FirstOrDefault(u => u.UserId == userId);
        if (user == null) return NotFound();

        if (user.PasswordHash != currentPassword)
        {
            ViewBag.Error = "Current password is incorrect.";
            return View();
        }

        if (newPassword != confirmPassword)
        {
            ViewBag.Error = "New passwords do not match.";
            return View();
        }

        user.PasswordHash = newPassword;
        _context.SaveChanges();

        TempData["Success"] = "Password changed successfully.";
        return RedirectToAction("Profile");
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        TempData["Success"] = "You have been logged out.";
        return RedirectToAction("Login");
    }

}
