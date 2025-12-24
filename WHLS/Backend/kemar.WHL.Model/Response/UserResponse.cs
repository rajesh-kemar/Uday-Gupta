using kemar.WHL.Model.Common;

namespace Kemar.WHL.Repository.Models.User
{
    public class UserResponse : CommonEntity
    {
        public int UserId { get; set; }
        public string Username { get; set; } = "";
        public string Email { get; set; } = "";
        public string Role { get; set; } = "";
        public IEnumerable<string>? Roles { get; set; } 
    }
}