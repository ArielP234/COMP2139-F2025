using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VirtualEventTicketing.Data;
using VirtualEventTicketing.Models;
using VirtualEventTicketing.Models.ViewModels;

namespace VirtualEventTicketing.Controllers
{
    public class EventsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EventsController(ApplicationDbContext context)
        {
            _context = context;
        }

       public async Task<IActionResult> Index(string searchString, int? categoryId, string sortOrder)
       {
           var events = _context.Events.Include(e => e.Category).AsQueryable();
       
           // REMOVE THIS LINE - no date filtering
           // var currentTime = DateTime.UtcNow;
           // events = events.Where(e => e.Date > currentTime);
       
           if (!string.IsNullOrEmpty(searchString))
           {
               events = events.Where(e => e.Title.Contains(searchString) || 
                                        e.Description.Contains(searchString));
           }
       
           if (categoryId.HasValue)
           {
               events = events.Where(e => e.CategoryId == categoryId.Value);
           }
       
           ViewData["TitleSort"] = string.IsNullOrEmpty(sortOrder) ? "title_desc" : "";
           ViewData["DateSort"] = sortOrder == "date_asc" ? "date_desc" : "date_asc";
           ViewData["PriceSort"] = sortOrder == "price_asc" ? "price_desc" : "price_asc";
       
           events = sortOrder switch
           {
               "title_desc" => events.OrderByDescending(e => e.Title),
               "date_asc" => events.OrderBy(e => e.Date),
               "date_desc" => events.OrderByDescending(e => e.Date),
               "price_asc" => events.OrderBy(e => e.TicketPrice),
               "price_desc" => events.OrderByDescending(e => e.TicketPrice),
               _ => events.OrderBy(e => e.Date)
           };
       
           var viewModel = new EventListViewModel
           {
               Events = await events.ToListAsync(),
               Categories = await _context.Categories.ToListAsync(),
               SearchString = searchString,
               CategoryId = categoryId,
               SortOrder = sortOrder
           };
       
           return View(viewModel);
       }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var @event = await _context.Events
                .Include(e => e.Category)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (@event == null) return NotFound();

            return View(@event);
        }

        public IActionResult Create()
        {
            ViewData["Categories"] = _context.Categories.ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Event @event)
        {
            if (ModelState.IsValid)
            {
                _context.Add(@event);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Categories"] = _context.Categories.ToList();
            return View(@event);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var @event = await _context.Events.FindAsync(id);
            if (@event == null) return NotFound();

            ViewData["Categories"] = _context.Categories.ToList();
            return View(@event);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Event @event)
        {
            if (id != @event.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(@event);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventExists(@event.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["Categories"] = _context.Categories.ToList();
            return View(@event);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var @event = await _context.Events
                .Include(e => e.Category)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (@event == null) return NotFound();

            return View(@event);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var @event = await _context.Events.FindAsync(id);
            if (@event != null)
            {
                _context.Events.Remove(@event);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Overview()
        {
            var viewModel = new EventOverviewViewModel
            {
                TotalEvents = await _context.Events.CountAsync(),
                TotalCategories = await _context.Categories.CountAsync(),
                EventsWithLowAvailability = await _context.Events
                    .Where(e => e.AvailableTickets < 5 && e.AvailableTickets > 0)
                    .Include(e => e.Category)
                    .ToListAsync(),
                SoldOutEvents = await _context.Events
                    .Where(e => e.AvailableTickets == 0)
                    .Include(e => e.Category)
                    .ToListAsync()
            };

            return View(viewModel);
        }

        private bool EventExists(int id) => _context.Events.Any(e => e.Id == id);
    }
}