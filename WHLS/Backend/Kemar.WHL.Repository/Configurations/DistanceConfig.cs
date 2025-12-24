using Kemar.WHL.Repository.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kemar.WHL.Repository.Configurations
{
    internal class DistanceConfig : BaseEntityConfig<Distance>
    {
        public override void Configure(EntityTypeBuilder<Distance> builder)
        {
            base.Configure(builder);
            builder.Property(x => x.DistanceId).ValueGeneratedOnAdd();
            builder.Property(x => x.Address).IsRequired().HasMaxLength(200);
            builder.Property(x => x.City).IsRequired().HasMaxLength(100);
            builder.Property(x => x.Country).IsRequired().HasMaxLength(100);
            builder.HasMany(x => x.Shipments).WithOne(x => x.Destination).HasForeignKey(x => x.DestinationId);
        }
    }
}