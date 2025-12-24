using Kemar.WHL.Repository.Entities.Base;

namespace Kemar.WHL.Repository.Entities
{
    public class Inventory : BaseEntity
    {
        public int InventoryId { get; set; }     
        public int Quantity { get; set; }
        public int WarehouseId { get; set; }     
        public Warehouse Warehouse { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
    }
}