using AutoMapper;
using kemar.WHL.Model.Filter;
using Kemar.WHL.Model.Common;
using Kemar.WHL.Model.Request;
using Kemar.WHL.Model.Response;

public class ShipmentService : IShipmentService
{
    private readonly IShipmentRepository _repo;

    public ShipmentService(IShipmentRepository repo)
    {
        _repo = repo;
    }

    public async Task<ResultModel> AddOrUpdateAsync(
        ShipmentRequest request,
        int userId,
        string username)
    {
        if (string.IsNullOrWhiteSpace(request.ShipmentNumber))
            return ResultModel.Invalid("Shipment number required");

        if (request.DestinationId <= 0 || request.VehicleId <= 0)
            return ResultModel.Invalid("Invalid destination or vehicle");

        // ================= CREATE =================
        if (request.ShipmentId == null)
        {
            var entity = new Shipment
            {
                ShipmentNumber = request.ShipmentNumber,
                ShipmentDate = request.ShipmentDate ?? DateTime.UtcNow,
                DestinationId = request.DestinationId,
                VehicleId = request.VehicleId,
                Status = request.Status ?? "CREATED",
                CreatedBy = username,
                CreatedAt = DateTime.UtcNow
            };

            await _repo.AddAsync(entity);
            return ResultModel.Created("Shipment created");
        }

        // ================= UPDATE =================
        var entityToUpdate = await _repo.GetEntityByIdAsync(request.ShipmentId.Value);
        if (entityToUpdate == null)
            return ResultModel.NotFound("Shipment not found");

        entityToUpdate.ShipmentNumber = request.ShipmentNumber;
        entityToUpdate.ShipmentDate = request.ShipmentDate ?? entityToUpdate.ShipmentDate;
        entityToUpdate.VehicleId = request.VehicleId;
        entityToUpdate.DestinationId = request.DestinationId;
        entityToUpdate.Status = request.Status; // ✅ THIS IS THE KEY FIX
        entityToUpdate.UpdatedBy = username;
        entityToUpdate.UpdatedAt = DateTime.UtcNow;

        await _repo.UpdateAsync(entityToUpdate);
        return ResultModel.Updated("Shipment updated");
    }

    public Task<ShipmentResponse?> GetByIdAsync(int id)
        => _repo.GetByIdAsync(id);

    public Task<List<ShipmentResponse>> GetByFilterAsync(ShipmentFilterModel filter)
        => _repo.GetByFilterAsync(filter);

    public Task<List<ShipmentResponse>> GetAllAsync()
        => _repo.GetAllAsync();

    public Task<bool> DeleteAsync(int id)
        => _repo.DeleteAsync(id, "system");
}
