namespace Logistics.Api.Models
{
    public class Vehicle
    {
        public int Id { get; set; }
        public string VehicleNumber { get; set; } = string.Empty; // registration
        public string Model { get; set; } = string.Empty;
        public int Capacity { get; set; } = 0;
        public bool IsAvailable { get; set; } = true;
    }
}