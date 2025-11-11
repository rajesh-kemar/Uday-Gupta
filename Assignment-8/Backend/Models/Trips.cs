namespace AdvLogisticSystem.Models
{
    public class Trip
    {
        public int Id { get; set; }
        public int DriverId { get; set; }
        public int VehicleId { get; set; }
        public string Origin { get; set; } = string.Empty;
        public string Destination { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string Status { get; set; } = "Active";

        public virtual Driver? Driver { get; set; }
        public virtual Vehicle? Vehicle { get; set; }
    }
}
