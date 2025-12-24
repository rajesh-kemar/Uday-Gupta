using kemar.WHL.Model.Common;

namespace Kemar.WHL.Model.Response
{
    public class WarehouseResponse : CommonEntity
    {
        public int WarehouseId { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public int Capacity { get; set; }
    }
}