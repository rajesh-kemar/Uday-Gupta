using kemar.WHL.Model.Common;
using System.ComponentModel.DataAnnotations;

namespace Kemar.WHL.Repository.Models.User
{
    public class UserRequest : CommonEntity
    {
        public int? UserId { get; set; }
        public List<int>? RoleIds { get; set; }

        [Required, MinLength(3), MaxLength(50)]
        public string Username { get; set; } = "";

        [Required, MaxLength(150)]
        public string Email { get; set; } = "";

        [Required, MinLength(6)]
        public string Password { get; set; } = "";

        [Required]
        public string Role { get; set; } = "User";
    }
}