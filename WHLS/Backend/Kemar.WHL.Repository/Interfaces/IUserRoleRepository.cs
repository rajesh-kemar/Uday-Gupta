using Kemar.WHL.Repository.Entities;

namespace Kemar.WHL.Repository.Interfaces
{
    public interface IUserRoleRepository
    {
        Task AssignRolesAsync(int userId, IEnumerable<int> roleIds);
        Task<IEnumerable<string>> GetUserRoleNamesAsync(int userId);
    }
}