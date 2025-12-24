using Kemar.WHL.Repository.Entities.Base;

namespace Kemar.WHL.Repository.Entities
{
    public class User : BaseEntity
    {
        public int UserId { get; set; }                
        public string Username { get; set; } = "";     
        public string Email { get; set; } = "";       
        public byte[] PasswordHash { get; set; }           
        public string Role { get; set; } = "User";
        public DateTime? LastActivityTime { get; set; }
        public int? WarehouseId { get; set; }
        public Warehouse? Warehouse { get; set; }
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
} 