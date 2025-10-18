using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VirtualEventTicketing.Data;
using VirtualEventTicketing.Models.ViewModels;

namespace VirtualEventTicketing.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ApplicationDbContext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var featuredEvents = await _context.Events
                .Include(e => e.Category)
                // REMOVE THIS LINE ↓
                // .Where(e => e.Date >= DateTime.Now)
                .OrderBy(e => e.Date)
                .Take(6)
                .ToListAsync();

            var viewModel = new HomeViewModel
            {
                FeaturedEvents = featuredEvents,
                // REMOVE THE DATE FILTER HERE TOO ↓
                TotalUpcomingEvents = await _context.Events.CountAsync(), // Count ALL events
                Categories = await _context.Categories.ToListAsync()
            };

            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
}