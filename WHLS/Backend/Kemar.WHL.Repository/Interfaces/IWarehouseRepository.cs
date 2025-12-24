using Kemar.WHL.Model.Request;
using Kemar.WHL.Model.Response;

namespace Kemar.WHL.Repository.Interfaces
{
    public interface IWarehouseRepository
    {
        Task<WarehouseResponse> AddAsync(WarehouseRequest request);                         
        Task<WarehouseResponse?> UpdateAsync(int id, WarehouseRequest request);             
        Task<IEnumerable<WarehouseResponse>> GetAllAsync();                                 
        Task<IEnumerable<WarehouseResponse>> GetByFilterAsync(WarehouseFilterModel filter); 
        Task<WarehouseResponse?> GetByIdAsync(int id);                                      
        Task<bool> DeleteAsync(int id);                                                     
    }
}