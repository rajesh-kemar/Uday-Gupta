using kemar.WHL.Model.Filter;
using Kemar.WHL.Model.Common;
using Kemar.WHL.Model.Request;
using Kemar.WHL.Model.Response;

public interface IShipmentService
{
    Task<ResultModel> AddOrUpdateAsync(ShipmentRequest request, int userId, string username);
    Task<ShipmentResponse?> GetByIdAsync(int id);
    Task<List<ShipmentResponse>> GetByFilterAsync(ShipmentFilterModel filter);
    Task<List<ShipmentResponse>> GetAllAsync();
    Task<bool> DeleteAsync(int id);
}