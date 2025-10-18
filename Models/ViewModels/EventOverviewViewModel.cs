using VirtualEventTicketing.Models;

namespace VirtualEventTicketing.Models.ViewModels
{
    public class EventOverviewViewModel
    {
        public int TotalEvents { get; set; }
        public int TotalCategories { get; set; }
        public List<Event> EventsWithLowAvailability { get; set; } = new();
        public List<Event> SoldOutEvents { get; set; } = new();
    }
}