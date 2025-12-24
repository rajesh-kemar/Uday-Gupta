using kemar.WHL.Model.Common;

namespace Kemar.WHL.Model.Response
{
    public class InventoryResponse : CommonEntity
    {
        public int InventoryId { get; set; }
        public int Quantity { get; set; }

        public int WarehouseId { get; set; }
        public string WarehouseName { get; set; }

        public int ProductId { get; set; }
        public string ProductName { get; set; }
    }
}
