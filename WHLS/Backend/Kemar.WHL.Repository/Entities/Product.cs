using Kemar.WHL.Repository.Entities.Base;

namespace Kemar.WHL.Repository.Entities
{
    public class Product : BaseEntity
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }  
        public decimal Weight { get; set; }
    }
}