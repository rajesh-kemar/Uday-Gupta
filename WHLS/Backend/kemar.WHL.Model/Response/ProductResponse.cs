using kemar.WHL.Model.Common;

namespace Kemar.WHL.Model.Response
{
    public class ProductResponse: CommonEntity
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Weight { get; set; }
    }
}