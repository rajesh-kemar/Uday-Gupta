namespace AdvLogisticSystem.DTOs
{
    public class CreateTripDto
    {
        public int DriverId { get; set; }
        public int VehicleId { get; set; }
        public string? Origin { get; set; }
        public string? Destination { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string? Status { get; set; }
    }
}