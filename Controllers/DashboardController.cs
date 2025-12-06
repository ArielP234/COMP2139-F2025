using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VirtualEventTicketing.Data;
using VirtualEventTicketing.Models;

namespace VirtualEventTicketing.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public DashboardController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            if (await _userManager.IsInRoleAsync(user, "Admin"))
                ViewBag.Role = "Admin";
            else if (await _userManager.IsInRoleAsync(user, "Organizer"))
                ViewBag.Role = "Organizer";
            else
                ViewBag.Role = "Attendee";

            return View();
        }

        public IActionResult MyTickets()
        {
            var userId = _userManager.GetUserId(User);

            var purchases = _context.Purchases
                .Include(p => p.PurchaseItems)
                    .ThenInclude(pi => pi.Event)
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.PurchaseDate)
                .ToList();

            return View(purchases);
        }

        public IActionResult PurchaseHistory()
        {
            var userId = _userManager.GetUserId(User);

            var purchases = _context.Purchases
                .Include(p => p.PurchaseItems)
                    .ThenInclude(pi => pi.Event)
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.PurchaseDate)
                .ToList();

            return View(purchases);
        }

        [Authorize(Roles = "Organizer,Admin")]
        public async Task<IActionResult> MyEvents()
        {
            var user = await _userManager.GetUserAsync(User);
            var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");

            var query = _context.Events.Include(e => e.Category).AsQueryable();

            if (!isAdmin)
            {
                query = query.Where(e => e.OrganizerId == user.Id);
            }

            var events = query.OrderBy(e => e.Date).ToList();
            return View(events);
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Profile(ApplicationUser model, IFormFile? profilePicture)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            if (ModelState.IsValid)
            {
                user.FullName = model.FullName;
                user.PhoneNumber = model.PhoneNumber;

                if (profilePicture != null && profilePicture.Length > 0)
                {
                    var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "profiles");
                    Directory.CreateDirectory(uploadsPath);

                    var fileName = $"{user.Id}_{Path.GetFileName(profilePicture.FileName)}";
                    var filePath = Path.Combine(uploadsPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await profilePicture.CopyToAsync(stream);
                    }

                    user.ProfilePicturePath = $"/uploads/profiles/{fileName}";
                }

                await _userManager.UpdateAsync(user);
                ViewBag.Success = "Profile updated.";
            }

            return View(user);
        }

        public IActionResult Analytics()
        {
            return View();
        }
    }
}