using Microsoft.EntityFrameworkCore;
using Logistics.Api.Models;

namespace Logistics.Api.Data
{
    public class LogisticsDbContext : DbContext
    {
        public LogisticsDbContext(DbContextOptions<LogisticsDbContext> options) : base(options) { }

        public DbSet<Driver> Drivers { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<Trip> Trips { get; set; }

        protected override void OnModelCreating(ModelBuilder mb)
        {
            base.OnModelCreating(mb);

            mb.Entity<Driver>().HasKey(d => d.Id);
            mb.Entity<Vehicle>().HasKey(v => v.Id);
            mb.Entity<Trip>().HasKey(t => t.Id);

            mb.Entity<Trip>()
              .HasOne(t => t.Driver)
              .WithMany()
              .HasForeignKey(t => t.DriverId)
              .OnDelete(DeleteBehavior.Restrict);

            mb.Entity<Trip>()
              .HasOne(t => t.Vehicle)
              .WithMany()
              .HasForeignKey(t => t.VehicleId)
              .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
