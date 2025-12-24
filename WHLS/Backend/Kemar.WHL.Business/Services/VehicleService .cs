using Kemar.WHL.Business.Interfaces;
using Kemar.WHL.Model.Common;
using Kemar.WHL.Model.Request;
using Kemar.WHL.Model.Response;
using Kemar.WHL.Repository.Interfaces;

namespace Kemar.WHL.Business.Services
{
    public class VehicleService : IVehicleService
    {
        private readonly IVehicleRepository _repo;

        public VehicleService(IVehicleRepository repo)
        {
            _repo = repo;
        }

        public async Task<ResultModel> AddOrUpdateAsync(VehicleRequest request, int userId, string username)
        {
            if (string.IsNullOrWhiteSpace(request.VehicleNumber))
                return ResultModel.Invalid("Vehicle number is required");

            if (request.Capacity <= 0)
                return ResultModel.Invalid("Capacity must be greater than zero");

            if (string.IsNullOrWhiteSpace(request.Type))
                return ResultModel.Invalid("Vehicle type is required");

            // ADD
            if (request.VehicleId == null || request.VehicleId == 0)
            {
                request.CreatedBy = username;
                request.CreatedAt = DateTime.UtcNow;

                request.UpdatedBy = username;
                request.UpdatedAt = DateTime.UtcNow;

                var added = await _repo.AddAsync(request);
                return ResultModel.Created("Vehicle added successfully", added);
            }

            // UPDATE
            request.UpdatedBy = username;
            request.UpdatedAt = DateTime.UtcNow;

            var updated = await _repo.UpdateAsync(request.VehicleId.Value, request);
            if (updated == null)
                return ResultModel.NotFound("Vehicle not found");

            return ResultModel.Updated("Vehicle updated successfully", updated);
        }

        public async Task<VehicleResponse?> GetByIdAsync(int id)
            => await _repo.GetByIdAsync(id);

        public async Task<IEnumerable<VehicleResponse>> GetAllAsync()
            => await _repo.GetAllAsync();

        public async Task<IEnumerable<VehicleResponse>> GetByFilterAsync(VehicleFilterRequest filter)
            => await _repo.GetByFilterAsync(filter);

        public async Task<bool> DeleteAsync(int id)
            => await _repo.DeleteAsync(id);
    }

}