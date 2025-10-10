using...
using COMP2139_F2025.Models;

namespace COMP2139_F2025.Data
{
    public class ApplicationDbContext : Dbco
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Project> Projects { get; set; }
    }
}
