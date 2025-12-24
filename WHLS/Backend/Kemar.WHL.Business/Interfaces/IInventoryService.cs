using Kemar.WHL.Model.Common;
using Kemar.WHL.Model.Request;
using Kemar.WHL.Model.Response;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IInventoryService
{
    Task<ResultModel> AddOrUpdateAsync(InventoryRequest request, string username);

    Task<InventoryResponse?> GetByIdAsync(int id);
    Task<IEnumerable<InventoryResponse>> GetAllAsync();
    Task<IEnumerable<InventoryResponse>> GetByFilterAsync(InventoryFilterModel filter);
    Task<bool> DeleteAsync(int id, string username);


}