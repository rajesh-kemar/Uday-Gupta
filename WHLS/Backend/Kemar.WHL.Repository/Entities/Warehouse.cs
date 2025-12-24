using Kemar.WHL.Repository.Entities.Base;

namespace Kemar.WHL.Repository.Entities
{
    public class Warehouse : BaseEntity
    {
        public int WarehouseId { get; set; }
        public string Name { get; set; }
        public string Location { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public ICollection<Inventory> Inventories { get; set; }
    }
}