using AdvLogisticSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace AdvLogisticSystem.Data
{
    public class LodisticsDbContext : DbContext
    {
        public LodisticsDbContext(DbContextOptions<LodisticsDbContext> options) : base(options) { }

        public DbSet<Driver> Drivers { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<Trip> Trips { get; set; }
        public DbSet<DriverTripSummary> DriverTripSummaries { get; set; }

        // 👇 Add this line for authentication system
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DriverTripSummary>().HasNoKey();
            base.OnModelCreating(modelBuilder);
        }
    }
}
