namespace TripApiEF.DTOs
{
    public class TripCreateDto
    {
        public int DriverId { get; set; }
        public int VehicleId { get; set; }

        public string Source { get; set; } = string.Empty;
        public string Destination { get; set; } = string.Empty;

        public DateTime StartingDate { get; set; }
        public DateTime EndingDate { get; set; }

        public bool IsCompleted { get; set; }
    }
}
