using Kemar.WHL.Repository.Entities;
using Kemar.WHL.Repository.Entities.Base;

public class Shipment : BaseEntity
{
    public int ShipmentId { get; set; }
    public string ShipmentNumber { get; set; }
    public DateTime ShipmentDate { get; set; }
    public int DestinationId { get; set; }
    public Distance Destination { get; set; } 
    public int VehicleId { get; set; }
    public Vehicle Vehicle { get; set; }
    public string Status { get; set; } = "CREATED";
}