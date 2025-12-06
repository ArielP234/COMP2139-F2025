using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VirtualEventTicketing.Data;

namespace VirtualEventTicketing.Controllers
{
    [Authorize(Roles = "Organizer,Admin")]
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class AnalyticsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AnalyticsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult TicketSalesByCategory()
        {
            var data = _context.PurchaseItems
                .Include(pi => pi.Event)
                .ThenInclude(e => e.Category)
                .GroupBy(pi => pi.Event.Category.Name)
                .Select(g => new
                {
                    Category = g.Key,
                    Tickets = g.Sum(x => x.Quantity)
                })
                .ToList();

            return Ok(data);
        }

        [HttpGet]
        public IActionResult RevenuePerMonth()
        {
            var data = _context.Purchases
                .GroupBy(p => new { p.PurchaseDate.Year, p.PurchaseDate.Month })
                .Select(g => new
                {
                    Month = $"{g.Key.Year}-{g.Key.Month:D2}",
                    Revenue = g.Sum(p => p.TotalAmount)
                })
                .OrderBy(x => x.Month)
                .ToList();

            return Ok(data);
        }
    }
}