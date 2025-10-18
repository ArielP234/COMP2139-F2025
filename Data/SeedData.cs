using VirtualEventTicketing.Models;

namespace VirtualEventTicketing.Data
{
    public static class SeedData
    {
        public static async Task Initialize(ApplicationDbContext context)
        {
            if (!context.Categories.Any())
            {
                var categories = new[]
                {
                    new Category { Name = "Webinar", Description = "Educational online sessions" },
                    new Category { Name = "Concert", Description = "Live music performances" },
                    new Category { Name = "Workshop", Description = "Interactive learning sessions" },
                    new Category { Name = "Conference", Description = "Professional gathering events" },
                    new Category { Name = "Meetup", Description = "Community networking events" }
                };

                await context.Categories.AddRangeAsync(categories);
                await context.SaveChangesAsync();
            }

            if (!context.Events.Any())
            {
                var categories = context.Categories.ToList();
                
                // In the events array, use DateTime.UtcNow
                var events = new[]
                {
                    new Event
                    {
                        Title = "ASP.NET Core Masterclass",
                        Description = "Deep dive into ASP.NET Core MVC and Entity Framework",
                        Date = DateTime.UtcNow.AddDays(7),  // Use UTC
                        TicketPrice = 79.99m,
                        AvailableTickets = 30,
                        CategoryId = categories.First(c => c.Name == "Workshop").Id
                    },
                    new Event
                    {
                        Title = "Virtual Jazz Festival",
                        Description = "Weekend-long virtual jazz experience",
                        Date = DateTime.UtcNow.AddDays(14),  // Use UTC
                        TicketPrice = 35.00m,
                        AvailableTickets = 2,
                        CategoryId = categories.First(c => c.Name == "Concert").Id
                    },
                    // ... other events
                };

                await context.Events.AddRangeAsync(events);
                await context.SaveChangesAsync();
            }
        }
    }
}