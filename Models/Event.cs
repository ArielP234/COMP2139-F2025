using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VirtualEventTicketing.Models
{
    public class Event
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Event title is required")]
        [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters")]
        public string Title { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Event date is required")]
        // REMOVE the FutureDate attribute temporarily
        public DateTime Date { get; set; }

        [Range(0, 10000, ErrorMessage = "Ticket price must be between 0 and 10,000")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TicketPrice { get; set; }

        [Range(0, 100000, ErrorMessage = "Available tickets must be between 0 and 100,000")]
        public int AvailableTickets { get; set; }

        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        public Category? Category { get; set; }
        public ICollection<PurchaseItem> PurchaseItems { get; set; } = new List<PurchaseItem>();
        
        public string? OrganizerId { get; set; }
        public ApplicationUser? Organizer { get; set; }
    }
    
}