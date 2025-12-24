using Kemar.WHL.Business.Interfaces;
using Kemar.WHL.Model.Common;
using Kemar.WHL.Model.Request;
using Kemar.WHL.Model.Response;
using Kemar.WHL.Repository.Interfaces;

namespace Kemar.WHL.Business.Services
{
    public class WarehouseService : IWarehouseService
    {
        private readonly IWarehouseRepository _repo;

        public WarehouseService(IWarehouseRepository repo)
        {
            _repo = repo;
        }

        // Add or Update
        public async Task<ResultModel> AddOrUpdateAsync(WarehouseRequest request, int userId, string username)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                return ResultModel.Invalid("Warehouse name is required");

            if (string.IsNullOrWhiteSpace(request.Location))
                return ResultModel.Invalid("Location is required");

            if (request.Capacity <= 0)
                return ResultModel.Invalid("Capacity must be greater than zero");

            if (request.WarehouseId == null || request.WarehouseId == 0)
            {
                // ADD
                request.CreatedBy = username;
                request.CreatedAt = DateTime.UtcNow;
                request.UpdatedBy = username;
                request.UpdatedAt = DateTime.UtcNow;

                var added = await _repo.AddAsync(request);
                return ResultModel.Created("Warehouse added successfully", added);
            }

            // UPDATE
            request.UpdatedBy = username;
            request.UpdatedAt = DateTime.UtcNow;

            var updated = await _repo.UpdateAsync(request.WarehouseId.Value, request);

            if (updated == null)
                return ResultModel.NotFound("Warehouse not found");

            return ResultModel.Updated("Warehouse updated successfully", updated);
        }

        // Get by Id
        public async Task<WarehouseResponse?> GetByIdAsync(int id)
        {
            return await _repo.GetByIdAsync(id);
        }

        // Get all warehouses
        public async Task<IEnumerable<WarehouseResponse>> GetAllAsync()
        {
            return await _repo.GetAllAsync();
        }

        // Get by filter
        public async Task<IEnumerable<WarehouseResponse>> GetByFilterAsync(WarehouseFilterModel filter)
        {
            return await _repo.GetByFilterAsync(filter);
        }

        // Delete 
        public async Task<bool> DeleteAsync(int id)
        {
            return await _repo.DeleteAsync(id);
        }
    }
}