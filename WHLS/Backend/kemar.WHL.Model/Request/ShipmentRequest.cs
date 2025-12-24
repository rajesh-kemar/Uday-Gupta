using System.ComponentModel.DataAnnotations;

namespace Kemar.WHL.Model.Request
{
    public class ShipmentRequest
    {
        public int? ShipmentId { get; set; }

        [Required, MinLength(3)]
        public string ShipmentNumber { get; set; } = null!;
        public DateTime? ShipmentDate { get; set; }

        [Required, Range(1, int.MaxValue)]
        public int DestinationId { get; set; }

        [Required, Range(1, int.MaxValue)]
        public int VehicleId { get; set; }
        public string Status { get; set; } = "CREATED";
    }
}