using VirtualEventTicketing.Models;

namespace VirtualEventTicketing.Models.ViewModels
{
    public class EventListViewModel
    {
        public List<Event> Events { get; set; } = new();
        public List<Category> Categories { get; set; } = new();
        public string? SearchString { get; set; }
        public int? CategoryId { get; set; }
        public string? SortOrder { get; set; }
    }
}