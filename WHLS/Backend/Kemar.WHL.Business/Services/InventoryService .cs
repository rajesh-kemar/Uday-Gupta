using Kemar.WHL.Model.Common;
using Kemar.WHL.Model.Request;
using Kemar.WHL.Model.Response;
using Kemar.WHL.Repository.Interfaces;
using Kemar.WHL.Repository.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class InventoryService : IInventoryService
{
    private readonly IInventoryRepository _repo;
    private readonly IUserRepository _userRepo;

    public InventoryService(IInventoryRepository repo, IUserRepository userRepo)
    {
        _repo = repo;
        _userRepo = userRepo;
    }

    public async Task<ResultModel> AddOrUpdateAsync(InventoryRequest request, string username)
    {
        if (request.Quantity <= 0)
            return ResultModel.Invalid("Quantity must be greater than zero.");

        var user = await _userRepo.GetByUsernameAsync(username);
        if (user == null)
            return ResultModel.Invalid("User not found.");

        // 🔐 Warehouse staff restriction
        if (user.Role == "WarehouseStaff" &&
            request.WarehouseId != user.WarehouseId)
            return ResultModel.Invalid("Unauthorized warehouse access.");

        var existing = await _repo.GetByWarehouseAndProductAsync(
            request.WarehouseId,
            request.ProductId);

        if (existing != null)
        {
            existing.Quantity += request.Quantity;
            existing.UpdatedBy = username;
            existing.UpdatedAt = DateTime.UtcNow;

            await _repo.UpdateEntityAsync(existing);
            return ResultModel.Updated("Inventory updated");
        }

        request.CreatedBy = username;
        request.CreatedAt = DateTime.UtcNow;
        request.UpdatedBy = username;
        request.UpdatedAt = DateTime.UtcNow;

        await _repo.AddAsync(request);
        return ResultModel.Created("Inventory created");
    }


    public async Task<InventoryResponse?> GetByIdAsync(int id) => await _repo.GetByIdAsync(id);

    public async Task<IEnumerable<InventoryResponse>> GetAllAsync() => await _repo.GetAllAsync();

    public async Task<IEnumerable<InventoryResponse>> GetByFilterAsync(InventoryFilterModel filter) => await _repo.GetByFilterAsync(filter);

    public async Task<bool> DeleteAsync(int id, string username) => await _repo.SoftDeleteAsync(id, username);
}
