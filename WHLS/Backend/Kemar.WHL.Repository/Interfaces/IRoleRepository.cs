using Kemar.WHL.Repository.Entities;

namespace Kemar.WHL.Repository.Interfaces
{
    public interface IRoleRepository
    {
        Task<Role?> GetByNameAsync(string name);
        Task<Role?> GetByIdAsync(int id);
        Task<IEnumerable<Role>> GetAllAsync();
    }

}