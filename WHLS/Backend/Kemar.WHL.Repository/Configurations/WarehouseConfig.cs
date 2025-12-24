using Kemar.WHL.Repository.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kemar.WHL.Repository.Configurations
{
    internal class WarehouseConfig : BaseEntityConfig<Warehouse>
    {
        public override void Configure(EntityTypeBuilder<Warehouse> builder)
        {
            base.Configure(builder);
            builder.Property(x => x.WarehouseId).ValueGeneratedOnAdd();
            builder.Property(x => x.Name).IsRequired().HasMaxLength(150);
            builder.Property(x => x.Location).HasMaxLength(300);
            builder.Property(x => x.Capacity).IsRequired();
            builder.HasMany(x => x.Inventories).WithOne(x => x.Warehouse).HasForeignKey(x => x.WarehouseId);
        }
    }
}