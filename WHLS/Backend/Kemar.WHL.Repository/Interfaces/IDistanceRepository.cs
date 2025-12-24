using Kemar.WHL.Model.Request;
using Kemar.WHL.Model.Response;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kemar.WHL.Repository.Interfaces
{
    public interface IDistanceRepository
    {
        Task<DistanceResponse> AddAsync(DistanceRequest request);
        Task<DistanceResponse?> UpdateAsync(int id, DistanceRequest request);
        Task<IEnumerable<DistanceResponse>> GetAllAsync();
        Task<IEnumerable<DistanceResponse>> GetByFilterAsync(DistanceFilterModel filter);
        Task<DistanceResponse?> GetByIdAsync(int id);

        Task<bool> SoftDeleteAsync(int id, string username);
    }
}