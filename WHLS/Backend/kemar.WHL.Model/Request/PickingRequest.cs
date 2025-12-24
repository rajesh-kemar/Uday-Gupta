using kemar.WHL.Model.Common;
using Kemar.WHL.Model.Common;
using System.ComponentModel.DataAnnotations;

public class PickingRequest : CommonEntity
{
    public int? PickingId { get; set; }

    [Required]
    public int ShipmentId { get; set; }

    [Required]
    public int ProductId { get; set; }

    [Required]
    public int WarehouseId { get; set; }

    [Required]
    [Range(1, int.MaxValue)]
    public int PickedQuantity { get; set; }
}
