using Microsoft.EntityFrameworkCore;
using TripApiEF.Models;

namespace TripApiEF.Data
{
    public class TripDbContext : DbContext
    {
        public TripDbContext(DbContextOptions<TripDbContext> options) : base(options) { }

        public DbSet<Driver> Drivers { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<Trip> Trips { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Optional: seed some data
            modelBuilder.Entity<Driver>().HasData(
                new Driver { Id = 1, Name = "John Doe" },
                new Driver { Id = 2, Name = "Alice Smith" }
            );

            modelBuilder.Entity<Vehicle>().HasData(
                new Vehicle { Id = 1, PlateNumber = "ABC123" },
                new Vehicle { Id = 2, PlateNumber = "XYZ789" }
            );

            modelBuilder.Entity<Trip>()
    .HasOne(t => t.Driver)
    .WithMany(d => d.Trips)
    .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
