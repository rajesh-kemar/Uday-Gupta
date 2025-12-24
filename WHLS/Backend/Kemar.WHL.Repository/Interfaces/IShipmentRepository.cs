using kemar.WHL.Model.Filter;
using Kemar.WHL.Model.Response;

public interface IShipmentRepository
{
    Task<Shipment> AddAsync(Shipment entity);
    Task<Shipment?> UpdateAsync(Shipment entity);
    Task<bool> DeleteAsync(int id, string username);
    Task<ShipmentResponse?> GetByIdAsync(int id);
    Task<List<ShipmentResponse>> GetByFilterAsync(ShipmentFilterModel filter);
    Task<List<ShipmentResponse>> GetAllAsync();
    Task<Shipment?> GetEntityByIdAsync(int id);
}
