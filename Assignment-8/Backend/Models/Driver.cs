namespace AdvLogisticSystem.Models
{
    public class Driver
    {
        public int Id { get; set; }
   
        public string Name { get; set; } = string.Empty;

        public string LicenseNumber { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public int Experience { get; set; } = 0;

        public bool IsAvailable { get; set; } = true;
    }
}
