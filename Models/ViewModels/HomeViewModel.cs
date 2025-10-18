using VirtualEventTicketing.Models;

namespace VirtualEventTicketing.Models.ViewModels
{
    public class HomeViewModel
    {
        public List<Event> FeaturedEvents { get; set; } = new();
        public int TotalUpcomingEvents { get; set; }
        public List<Category> Categories { get; set; } = new();
    }
}