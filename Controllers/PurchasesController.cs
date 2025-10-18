using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VirtualEventTicketing.Data;
using VirtualEventTicketing.Models;
using VirtualEventTicketing.Models.ViewModels;

namespace VirtualEventTicketing.Controllers
{
    public class PurchasesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PurchasesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Create(int eventId)
        {
            var @event = await _context.Events.FindAsync(eventId);
            if (@event == null) return NotFound();

            var viewModel = new PurchaseCreateViewModel
            {
                EventId = eventId,
                Event = @event,
                Quantity = 1
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PurchaseCreateViewModel viewModel)
        {
            var @event = await _context.Events.FindAsync(viewModel.EventId);
            if (@event == null) return NotFound();

            if (viewModel.Quantity > @event.AvailableTickets)
            {
                ModelState.AddModelError("Quantity", "Not enough tickets available.");
            }

            if (ModelState.IsValid)
            {
                var purchase = new Purchase
                {
                    CustomerName = viewModel.CustomerName,
                    CustomerEmail = viewModel.CustomerEmail,
                    PurchaseDate = DateTime.Now,
                    TotalAmount = viewModel.Quantity * @event.TicketPrice
                };

                var purchaseItem = new PurchaseItem
                {
                    EventId = viewModel.EventId,
                    Quantity = viewModel.Quantity,
                    UnitPrice = @event.TicketPrice
                };

                purchase.PurchaseItems.Add(purchaseItem);
                @event.AvailableTickets -= viewModel.Quantity;

                _context.Purchases.Add(purchase);
                await _context.SaveChangesAsync();

                return RedirectToAction("Confirmation", new { id = purchase.Id });
            }

            viewModel.Event = @event;
            return View(viewModel);
        }

        public async Task<IActionResult> Confirmation(int id)
        {
            var purchase = await _context.Purchases
                .Include(p => p.PurchaseItems)
                .ThenInclude(pi => pi.Event)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (purchase == null) return NotFound();

            return View(purchase);
        }
    }
}