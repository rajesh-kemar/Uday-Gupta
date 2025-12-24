using Kemar.WHL.Model.Common;
using Kemar.WHL.Model.Request;
using Kemar.WHL.Repository.Interfaces;

public class PickingService : IPickingService
{
    private readonly IPickingRepository _pickingRepo;
    private readonly IInventoryRepository _inventoryRepo;

    public PickingService(
        IPickingRepository pickingRepo,
        IInventoryRepository inventoryRepo)
    {
        _pickingRepo = pickingRepo;
        _inventoryRepo = inventoryRepo;
    }

    public async Task<ResultModel> PickAsync(PickingRequest request, string username)
    {
        // 1️⃣ Get inventory (ENTITY)
        var inventory = await _inventoryRepo.GetByWarehouseAndProductAsync(
            request.WarehouseId,
            request.ProductId);

        if (inventory == null)
            return ResultModel.Invalid("Inventory not found");

        if (inventory.Quantity < request.PickedQuantity)
            return ResultModel.Invalid("Insufficient stock");

        // 2️⃣ Reduce stock
        inventory.Quantity -= request.PickedQuantity;
        inventory.UpdatedBy = username;
        inventory.UpdatedAt = DateTime.UtcNow;

        await _inventoryRepo.UpdateEntityAsync(inventory);

        // 3️⃣ Save picking record
        request.CreatedBy = username;
        request.CreatedAt = DateTime.UtcNow;

        await _pickingRepo.AddAsync(request);

        return ResultModel.Created("Picking completed successfully");
    }

    public async Task<IEnumerable<PickingResponse>> GetByShipmentAsync(int shipmentId)
    {
        return await _pickingRepo.GetByShipmentAsync(shipmentId);
    }
}
