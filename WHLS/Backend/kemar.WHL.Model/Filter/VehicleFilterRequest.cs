namespace Kemar.WHL.Model.Request
{
    public class VehicleFilterRequest
    {
        public string? VehicleNumber { get; set; }
        public string? Type { get; set; }
        public decimal? MinCapacity { get; set; }
        public decimal? MaxCapacity { get; set; }
    }
}