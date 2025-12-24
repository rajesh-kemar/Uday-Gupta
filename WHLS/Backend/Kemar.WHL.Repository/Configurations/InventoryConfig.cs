using Kemar.WHL.Repository.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kemar.WHL.Repository.Configurations
{
    internal class InventoryConfig : BaseEntityConfig<Inventory>
    {
        public override void Configure(EntityTypeBuilder<Inventory> builder)
        {
            base.Configure(builder);
            builder.Property(x => x.InventoryId).ValueGeneratedOnAdd();
            builder.Property(x => x.Quantity).IsRequired();
            builder.HasOne(x => x.Warehouse).WithMany(x => x.Inventories).HasForeignKey(x => x.WarehouseId);
            builder.HasOne(x => x.Product)
        .WithMany()
        .HasForeignKey(x => x.ProductId);

        }
    }
}