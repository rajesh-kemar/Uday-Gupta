using Kemar.WHL.Repository.Entities.Base;

namespace Kemar.WHL.Repository.Entities
{
    public class Vehicle : BaseEntity
    {
        public int VehicleId { get; set; }     
        public string VehicleNumber { get; set; }  
        public decimal Capacity { get; set; }
        public string Type { get; set; }
        public ICollection<Shipment> Shipments { get; set; } = new List<Shipment>();
    }
}