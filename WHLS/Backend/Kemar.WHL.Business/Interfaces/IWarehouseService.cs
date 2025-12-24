using Kemar.WHL.Model.Common;
using Kemar.WHL.Model.Request;
using Kemar.WHL.Model.Response;

namespace Kemar.WHL.Business.Interfaces
{
    public interface IWarehouseService
    {
        Task<ResultModel> AddOrUpdateAsync(WarehouseRequest request, int userId, string username);
        Task<WarehouseResponse?> GetByIdAsync(int id);                
        Task<IEnumerable<WarehouseResponse>> GetAllAsync();           
        Task<IEnumerable<WarehouseResponse>> GetByFilterAsync(WarehouseFilterModel filter); 
        Task<bool> DeleteAsync(int id);                              
    }
}