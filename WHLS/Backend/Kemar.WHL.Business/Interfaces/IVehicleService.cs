using Kemar.WHL.Model.Common;
using Kemar.WHL.Model.Request;
using Kemar.WHL.Model.Response;

namespace Kemar.WHL.Business.Interfaces
{
    public interface IVehicleService
    {
        Task<ResultModel> AddOrUpdateAsync(VehicleRequest request, int userId, string username);
        Task<VehicleResponse?> GetByIdAsync(int id);
        Task<IEnumerable<VehicleResponse>> GetAllAsync();
        Task<IEnumerable<VehicleResponse>> GetByFilterAsync(VehicleFilterRequest filter);
        Task<bool> DeleteAsync(int id);
    }

}