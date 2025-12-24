using kemar.WHL.Model.Filter;
using Kemar.WHL.Model.Common;
using Kemar.WHL.Repository.Models.User;

namespace Kemar.WHL.Business.Interfaces
{
    public interface IUserService
    {
        Task<ResultModel> AddOrUpdateAsync(UserRequest request, int loggedInUserId);
        Task<IEnumerable<UserResponse>> GetAllAsync();
        Task<IEnumerable<UserResponse>> GetByFilterAsync(UserFilterModel filter);
        Task<UserResponse?> GetByIdAsync(int id);
        Task<bool> DeleteAsync(int id, int loggedInUserId);
    }
}