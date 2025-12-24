using kemar.WHL.Model.Filter;
using Kemar.WHL.Repository.Entities;
using Kemar.WHL.Repository.Models.User;

namespace Kemar.WHL.Repository.Interfaces
{
    public interface IUserRepository
    {
        Task<UserResponse> AddAsync(UserRequest request);
        Task<UserResponse?> UpdateAsync(int id, UserRequest request);
        Task<IEnumerable<UserResponse>> GetAllAsync();
        Task<IEnumerable<UserResponse>> GetByFilterAsync(UserFilterModel filter);
        Task<UserResponse?> GetByIdAsync(int id);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<string>> GetUserRoleNamesAsync(int userId);
        Task<User?> GetEntityByIdAsync(int id);
        Task<User?> GetEntityByEmailAsync(string email);
        Task<User?> GetByUsernameAsync(string username);
        Task UpdatePasswordAsync(User entity);
        Task UpdateLastActivityAsync(int userId, DateTime time);
    }
}