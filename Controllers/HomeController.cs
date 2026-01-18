using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CsrfDemo.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return View();

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, username)
            };

            var identity = new ClaimsIdentity(claims, "CookieAuth");
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync("CookieAuth", principal);

            return RedirectToAction("Profile");
        }

        [Authorize]
        public IActionResult Profile()
        {
            ViewBag.Email = $"{User.Identity?.Name ?? "unknown"}@example.com";
            ViewBag.Message = TempData["Message"] as string;
            return View();
        }

        // вразлива дія — без [ValidateAntiForgeryToken]
        [HttpPost]
        // [ValidateAntiForgeryToken]
        public IActionResult ChangeEmail(string newEmail)
        {
            if (!string.IsNullOrWhiteSpace(newEmail))
            {
                ViewBag.Email = newEmail;
                ViewBag.Message = $"Email успішно змінено на: {newEmail}";
            }
            else
            {
                ViewBag.Message = "Вкажіть email";
            }

            return View("Profile");
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("CookieAuth");
            return RedirectToAction("Index");
        }
    }
}