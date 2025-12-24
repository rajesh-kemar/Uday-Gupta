using kemar.WHL.Model.Common;
using Kemar.WHL.Model.Common;

public class PickingResponse : CommonEntity
{
    public int PickingId { get; set; }

    public int ShipmentId { get; set; }

    public int ProductId { get; set; }
    public string ProductName { get; set; }

    public int WarehouseId { get; set; }
    public string WarehouseName { get; set; }

    public int PickedQuantity { get; set; }
}
