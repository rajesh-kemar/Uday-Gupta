using Kemar.WHL.Model.Common;
using Kemar.WHL.Model.Request;
using Kemar.WHL.Model.Response;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kemar.WHL.Business.Interfaces
{
    public interface IDistanceService
    {
        Task<ResultModel> AddOrUpdateAsync(DistanceRequest request, string username);
        Task<DistanceResponse?> GetByIdAsync(int id);
        Task<IEnumerable<DistanceResponse>> GetAllAsync();
        Task<IEnumerable<DistanceResponse>> GetByFilterAsync(DistanceFilterModel filter);
        Task<bool> DeleteAsync(int id, string username);
    }
}