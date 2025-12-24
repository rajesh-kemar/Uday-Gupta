using Kemar.WHL.Repository.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kemar.WHL.Repository.Configurations
{
    internal class VehicleConfig : BaseEntityConfig<Vehicle>
    {
        public override void Configure(EntityTypeBuilder<Vehicle> builder)
        {
            base.Configure(builder);
            builder.Property(x => x.VehicleId).ValueGeneratedOnAdd();
            builder.Property(x => x.VehicleNumber).IsRequired().HasMaxLength(50);
            builder.Property(x => x.Type).IsRequired().HasMaxLength(50);
            builder.Property(x => x.Capacity).HasColumnType("decimal(10,2)");
        }
    }
}