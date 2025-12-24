namespace Kemar.WHL.Model.Request
{
    public class ProductFilterRequest
    {
        public string? Name { get; set; }
        public decimal? MinWeight { get; set; }
        public decimal? MaxWeight { get; set; }
    }
}