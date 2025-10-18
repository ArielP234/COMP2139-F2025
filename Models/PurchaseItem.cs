using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VirtualEventTicketing.Models
{
    public class PurchaseItem
    {
        public int Id { get; set; }
        
        [Range(1, 100, ErrorMessage = "Quantity must be between 1 and 100")]
        public int Quantity { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }

        public int PurchaseId { get; set; }
        public int EventId { get; set; }

        public Purchase Purchase { get; set; } = null!;
        public Event Event { get; set; } = null!;
    }
}