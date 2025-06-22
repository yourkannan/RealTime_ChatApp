using Microsoft.AspNetCore.Mvc;

namespace CHAT.Controllers
{
    public class ChatController : Controller
    {
        public IActionResult Index()
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
                return RedirectToAction("Login", "Auth");

            ViewData["Username"] = HttpContext.Session.GetString("Username");
            return View();
        }
    }
}