using CharityApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MailKit.Net.Smtp;
using MimeKit;

namespace CharityApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly CharityDbContext _context;
        private readonly EmailService _emailService;

        public AccountController(CharityDbContext context, EmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email && u.PasswordHash == model.Password);

                if (user != null)
                {
                   
                    // Set session values
                    HttpContext.Session.SetString("UserRole", user.Role);
                    HttpContext.Session.SetInt32("UserId", user.UserId);
                    HttpContext.Session.SetString("FullName", user.FullName); // <-- ADDED HERE


                    if (user.Role == "Admin")
                        return RedirectToAction("Admin", "Home");
                    else
                        return RedirectToAction("User", "Home");
                }
                
                ModelState.AddModelError("", "Invalid email or password.");
            }


            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var existing = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
                if (existing != null)
                {
                    ModelState.AddModelError("", "Email is already registered.");
                    return View(model);
                }

                var user = new User
                {
                    FullName = model.Name,
                    Email = model.Email,
                    PasswordHash = model.Password, // In production, hash this
                    Role = "User",
                    CreatedAt = DateTime.Now
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                TempData["Message"] = "Registered successfully!";
                return RedirectToAction("Login");
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
                if (user != null)
                {
                    // Generate reset token and expiration
                    user.ResetToken = Guid.NewGuid().ToString();
                    user.ResetTokenExpiration = DateTime.Now.AddHours(1);
                    await _context.SaveChangesAsync();

                    // Generate reset link
                    var resetLink = Url.Action("ResetPassword", "Account", new { token = user.ResetToken }, Request.Scheme);

                    // Send email using EmailService
                    string subject = "Reset your password";
                    string body = $"Click the link to reset your password: {resetLink}";

                    try
                    {
                        _emailService.SendEmail(user.Email, subject, body);
                        TempData["Message"] = "Reset link sent to your email.";
                    }
                    catch (Exception ex)
                    {
                        TempData["Error"] = "Error sending email: " + ex.Message;
                    }

                    return RedirectToAction("Login");
                }

                ModelState.AddModelError("", "No account found with this email.");
            }

            return View(model);
        }


        [HttpGet]
        public async Task<IActionResult> ResetPassword(string token)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.ResetToken == token && u.ResetTokenExpiration > DateTime.Now);
            if (user == null)
            {
                return RedirectToAction("ForgotPassword");
            }

            var model = new ResetPasswordViewModel { Token = token };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.ResetToken == model.Token && u.ResetTokenExpiration > DateTime.Now);
                if (user != null)
                {
                    user.PasswordHash = model.NewPassword; // In production, hash it
                    user.ResetToken = null;
                    user.ResetTokenExpiration = null;
                    await _context.SaveChangesAsync();

                    TempData["Message"] = "Password reset successfully!";
                    return RedirectToAction("Login");
                }

                ModelState.AddModelError("", "Invalid or expired token.");
            }

            return View(model);
        }

        public IActionResult Logout()
        {
            
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
