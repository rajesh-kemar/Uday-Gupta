using kemar.WHL.Model.Common;
using System.ComponentModel.DataAnnotations;

namespace Kemar.WHL.Model.Request
{
    public class VehicleRequest : CommonEntity
    {
        public int? VehicleId { get; set; }

        [Required, MinLength(3)]
        public string VehicleNumber { get; set; }

        [Required, Range(0.01, double.MaxValue)]
        public decimal Capacity { get; set; }

        [Required, MinLength(3)]
        public string Type { get; set; }
    }
}