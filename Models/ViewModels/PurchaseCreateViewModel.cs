using System.ComponentModel.DataAnnotations;

namespace VirtualEventTicketing.Models.ViewModels
{
    public class PurchaseCreateViewModel
    {
        public int EventId { get; set; }
        public Event? Event { get; set; }

        [Range(1, 100, ErrorMessage = "Quantity must be between 1 and 100")]
        public int Quantity { get; set; } = 1;

        [Required(ErrorMessage = "Customer name is required")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string CustomerName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
        public string CustomerEmail { get; set; } = string.Empty;
    }
}