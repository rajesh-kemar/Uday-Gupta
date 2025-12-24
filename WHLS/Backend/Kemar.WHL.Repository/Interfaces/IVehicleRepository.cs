using Kemar.WHL.Model.Request;
using Kemar.WHL.Model.Response;

namespace Kemar.WHL.Repository.Interfaces
{
    public interface IVehicleRepository
    {
        Task<VehicleResponse> AddAsync(VehicleRequest request);
        Task<VehicleResponse?> UpdateAsync(int id, VehicleRequest request);
        Task<IEnumerable<VehicleResponse>> GetAllAsync();
        Task<IEnumerable<VehicleResponse>> GetByFilterAsync(VehicleFilterRequest filter);
        Task<VehicleResponse?> GetByIdAsync(int id);
        Task<bool> DeleteAsync(int id);
    }
}