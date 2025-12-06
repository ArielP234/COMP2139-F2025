using Microsoft.AspNetCore.Mvc;
using VirtualEventTicketing.Data;

namespace VirtualEventTicketing.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                ViewBag.UserName = User.Identity.Name;

                var events = _context.Events
                    .OrderBy(e => e.Date)
                    .Take(3)
                    .ToList();

                return View("IndexLoggedIn", events);
            }

            return View("IndexLoggedOut");
        }

        public IActionResult Privacy() => View();

        public IActionResult StatusCode(int code)
        {
            if (code == 404)
                return View("NotFound");

            return View("Error");
        }
    }
}