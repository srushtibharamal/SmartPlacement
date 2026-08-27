using Microsoft.EntityFrameworkCore;

namespace SmartPlacement.Models
{
    public class SmartPlacementContext : DbContext
    {
        public SmartPlacementContext(
            DbContextOptions<SmartPlacementContext> options)
            : base(options)
        {
        }

        public DbSet<Student> Students { get; set; }

        public DbSet<Job> Jobs { get; set; }

        public DbSet<Application> Applications { get; set; }

        public DbSet<Company> Companies { get; set; }
    }
}