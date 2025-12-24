using kemar.WHL.Model.Common;
using System.ComponentModel.DataAnnotations;

namespace Kemar.WHL.Model.Request
{
    public class DistanceRequest : CommonEntity
    {
        public int? DistanceId { get; set; }

        [Required, MinLength(3)]
        public string Address { get; set; }

        [Required, MinLength(2)]
        public string City { get; set; }

        [Required, MinLength(2)]
        public string Country { get; set; }
    }
}