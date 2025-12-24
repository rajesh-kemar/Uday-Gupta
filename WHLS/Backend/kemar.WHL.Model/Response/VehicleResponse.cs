using kemar.WHL.Model.Common;

namespace Kemar.WHL.Model.Response
{
    public class VehicleResponse: CommonEntity
    {
        public int VehicleId { get; set; }
        public string VehicleNumber { get; set; }
        public decimal Capacity { get; set; }
        public string Type { get; set; }
    }
}