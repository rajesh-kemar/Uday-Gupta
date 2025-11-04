using System;

namespace TripApiEF.Models
{
    public class Trip
    {
        public int Id { get; set; }

        public string Source { get; set; } = string.Empty;
        public string Destination { get; set; } = string.Empty;

        public DateTime StartingDate { get; set; }
        public DateTime EndingDate { get; set; }

        public bool IsCompleted { get; set; }

        public int DriverId { get; set; }
        public Driver? Driver { get; set; }

        public int VehicleId { get; set; }
        public Vehicle? Vehicle { get; set; }
    }
}
