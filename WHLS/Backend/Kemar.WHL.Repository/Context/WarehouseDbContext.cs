using Kemar.WHL.Repository.Configurations;
using Kemar.WHL.Repository.Entities;
using Kemar.WHL.Repository.Entities.Base;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Kemar.WHL.Repository.Context
{
    public class WarehouseDbContext : DbContext
    {
        public WarehouseDbContext(DbContextOptions<WarehouseDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Shipment> Shipments { get; set; }
        public DbSet<Distance> Distances { get; set; }
        public DbSet<Warehouse> Warehouses { get; set; }
        public DbSet<Inventory> Inventories { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Picking> Pickings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Keys
            modelBuilder.Entity<Product>().HasKey(i => i.ProductId);
            modelBuilder.Entity<Warehouse>().HasKey(w => w.WarehouseId);
            modelBuilder.Entity<Inventory>().HasKey(i => i.InventoryId);
            modelBuilder.Entity<Distance>().HasKey(d => d.DistanceId);
            modelBuilder.Entity<Vehicle>().HasKey(v => v.VehicleId);
            modelBuilder.Entity<Shipment>().HasKey(s => s.ShipmentId);

            // Precision
            modelBuilder.Entity<Product>().Property(i => i.Weight).HasPrecision(18, 2);
            modelBuilder.Entity<Vehicle>().Property(v => v.Capacity).HasPrecision(18, 2);

            modelBuilder.ApplyConfiguration(new UserConfiguration());

            // Inventory → Warehouse
            modelBuilder.Entity<Inventory>()
                .HasOne(i => i.Warehouse)
                .WithMany(w => w.Inventories)
                .HasForeignKey(i => i.WarehouseId);

            // Inventory → Product (Item)
            modelBuilder.Entity<Inventory>()
                .HasOne(i => i.Product)
                .WithMany()
                .HasForeignKey(i => i.ProductId);

            // Shipment → Vehicle
            modelBuilder.Entity<Shipment>()
                .HasOne(s => s.Vehicle)
                .WithMany(v => v.Shipments)
                .HasForeignKey(s => s.VehicleId);

            // Shipment → Destination (Distance)
            modelBuilder.Entity<Shipment>()
                .HasOne(s => s.Destination)
                .WithMany(d => d.Shipments)
                .HasForeignKey(s => s.DestinationId);

            // UserRole composite key & relationships
            modelBuilder.Entity<UserRole>().HasKey(ur => new { ur.UserId, ur.RoleId });

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
    .HasOne(u => u.Warehouse)
    .WithMany()
    .HasForeignKey(u => u.WarehouseId)
    .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Picking>().HasKey(p => p.PickingId);

            modelBuilder.Entity<Picking>()
                .HasOne(p => p.Product)
                .WithMany()
                .HasForeignKey(p => p.ProductId);

            modelBuilder.Entity<Picking>()
                .HasOne(p => p.Warehouse)
                .WithMany()
                .HasForeignKey(p => p.WarehouseId);

            modelBuilder.Entity<Picking>()
                .HasOne(p => p.Shipment)
                .WithMany()
                .HasForeignKey(p => p.ShipmentId);

           

            // Seed default roles (IDs stable)
            modelBuilder.Entity<Role>().HasData(
                new Role { RoleId = 1, Name = "Admin" },
                new Role { RoleId = 2, Name = "TransportAdmin" },
                new Role { RoleId = 3, Name = "WarehouseStaff" },
                new Role { RoleId = 4, Name = "DeliveryStaff" }
            );

            // Soft Delete Filter for all BaseEntity
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var clrType = entityType.ClrType;

                if (typeof(BaseEntity).IsAssignableFrom(clrType) && clrType != typeof(BaseEntity))
                {
                    var parameter = Expression.Parameter(clrType, "e");
                    var isDeletedProperty = Expression.Property(parameter, nameof(BaseEntity.IsDeleted));
                    var isDeletedFalse = Expression.Equal(isDeletedProperty, Expression.Constant(false));
                    var lambda = Expression.Lambda(isDeletedFalse, parameter);

                    modelBuilder.Entity(clrType).HasQueryFilter(lambda);
                }
            }
        }
    }
}