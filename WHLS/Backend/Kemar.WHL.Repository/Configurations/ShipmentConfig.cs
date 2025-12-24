using Kemar.WHL.Repository.Configurations;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal class ShipmentConfig : BaseEntityConfig<Shipment>
{
    public override void Configure(EntityTypeBuilder<Shipment> builder)
    {
        builder.Property(x => x.ShipmentId).ValueGeneratedOnAdd();
        builder.HasKey(x => x.ShipmentId);

        builder.Property(x => x.ShipmentNumber)
               .IsRequired()
               .HasMaxLength(50);

        builder.Property(x => x.ShipmentDate).IsRequired();

        builder.HasOne(x => x.Destination)
               .WithMany(x => x.Shipments)
               .HasForeignKey(x => x.DestinationId);

        builder.HasOne(x => x.Vehicle)
               .WithMany(x => x.Shipments)
               .HasForeignKey(x => x.VehicleId);
    }
}