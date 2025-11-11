namespace AdvLogisticSystem.Models
{
    public class Vehicle
    {
        public int Id { get; set; }
        public string RegistrationNumber { get; set; } = null!;
        public string Model { get; set; } = null!;

        public bool IsAvailable { get; set; } = true;
    }
}
