using Kemar.WHL.Model.Common;

public interface IPickingService
{
    Task<ResultModel> PickAsync(PickingRequest request, string username);
    Task<IEnumerable<PickingResponse>> GetByShipmentAsync(int shipmentId);
}
