using Kemar.WHL.Repository.Entities.Base;

namespace Kemar.WHL.Repository.Entities
{
    public class Distance : BaseEntity
    {
        public int DistanceId { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public ICollection<Shipment> Shipments { get; set; }
    }
}