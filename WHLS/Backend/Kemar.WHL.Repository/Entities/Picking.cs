using Kemar.WHL.Repository.Entities;
using Kemar.WHL.Repository.Entities.Base;

public class Picking : BaseEntity
{
    public int PickingId { get; set; }

    public int ShipmentId { get; set; }
    public Shipment Shipment { get; set; } = null!;

    public int WarehouseId { get; set; }
    public Warehouse Warehouse { get; set; } = null!;

    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public int PickedQuantity { get; set; }
}