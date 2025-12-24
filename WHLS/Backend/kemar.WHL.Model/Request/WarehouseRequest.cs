using kemar.WHL.Model.Common;
using System.ComponentModel.DataAnnotations;

namespace Kemar.WHL.Model.Request
{
    public class WarehouseRequest : CommonEntity
    {
        public int? WarehouseId { get; set; }

        [Required, MinLength(2)]
        public string Name { get; set; }

        [Required, MinLength(3)]
        public string Location { get; set; }

        [Required, Range(1, long.MaxValue)] 
        public long Capacity { get; set; }
    }
}