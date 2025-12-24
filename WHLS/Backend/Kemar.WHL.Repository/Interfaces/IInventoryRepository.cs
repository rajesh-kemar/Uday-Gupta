using Kemar.WHL.Model.Request;
using Kemar.WHL.Model.Response;
using Kemar.WHL.Repository.Entities;

public interface IInventoryRepository
{
    Task<InventoryResponse> AddAsync(InventoryRequest request);
    Task<InventoryResponse?> UpdateAsync(int id, InventoryRequest request);
    Task<IEnumerable<InventoryResponse>> GetAllAsync();
    Task<IEnumerable<InventoryResponse>> GetByFilterAsync(InventoryFilterModel filter);
    Task<InventoryResponse?> GetByIdAsync(int id);
    Task<bool> SoftDeleteAsync(int id, string username);

    Task<Inventory?> GetByWarehouseAndProductAsync(int warehouseId, int productId);
    Task UpdateEntityAsync(Inventory inventory);
}
