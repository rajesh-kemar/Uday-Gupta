using Kemar.WHL.Model.Request;
using Kemar.WHL.Model.Response;

public interface IPickingRepository
{
    Task<PickingResponse> AddAsync(PickingRequest request);
    Task<List<PickingResponse>> GetByShipmentAsync(int shipmentId);
}
