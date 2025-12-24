using kemar.WHL.Model.Common;

namespace Kemar.WHL.Model.Response
{
    public class ShipmentResponse : CommonEntity
    {
        public int ShipmentId { get; set; }

        public string ShipmentNumber { get; set; } = "";

        public DateTime ShipmentDate { get; set; }

        public int DestinationId { get; set; }
        public string DestinationAddress { get; set; } = "";

        public int VehicleId { get; set; }
        public string VehicleNumber { get; set; } = "";

        public string Status { get; set; } = "";
    }
}