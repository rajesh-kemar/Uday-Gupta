using kemar.WHL.Model.Common;
using System.ComponentModel.DataAnnotations;

namespace Kemar.WHL.Model.Request
{
    public class InventoryRequest : CommonEntity 
    {
        public int? InventoryId { get; set; }  

        [Required, Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [Required, Range(1, int.MaxValue)]
        public int WarehouseId { get; set; }

        [Required, Range(1, int.MaxValue)]
        public int ProductId { get; set; }
    }
}