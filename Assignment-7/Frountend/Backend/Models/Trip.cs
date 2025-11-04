using System;

namespace Logistics.Api.Models
{
    public class Trip
    {
        public int Id { get; set; }

        public int DriverId { get; set; }
        public Driver? Driver { get; set; }

        public int VehicleId { get; set; }
        public Vehicle? Vehicle { get; set; }

        public string Origin { get; set; } = string.Empty;
        public string Destination { get; set; } = string.Empty;

        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }

        // ✅ Status property linked to TripStatus enum
        public TripStatus Status { get; set; } = TripStatus.Planned;

        // ✅ Duration auto-calculated
        public TimeSpan? Duration =>
            (StartTime.HasValue && EndTime.HasValue)
                ? EndTime - StartTime
                : null;
    }
}
