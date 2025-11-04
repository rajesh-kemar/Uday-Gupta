namespace Logistics.Api.DTOs
{
    public class CreateTripDto
    {
        public int DriverId { get; set; }
        public int VehicleId { get; set; }
        public string Origin { get; set; } = string.Empty;
        public string Destination { get; set; } = string.Empty;
        public DateTime? StartTime { get; set; }   // ✅ Must exist
        public DateTime? EndTime { get; set; }     // ✅ Must exist
    }
}
