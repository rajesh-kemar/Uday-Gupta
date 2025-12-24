using Kemar.WHL.Business.Interfaces;
using Kemar.WHL.Model.Common;
using Kemar.WHL.Model.Request;
using Kemar.WHL.Model.Response;
using Kemar.WHL.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kemar.WHL.Business.Services
{
    public class DistanceService : IDistanceService
    {
        private readonly IDistanceRepository _repo;

        public DistanceService(IDistanceRepository repo)
        {
            _repo = repo;
        }

        public async Task<ResultModel> AddOrUpdateAsync(DistanceRequest request, string username)
        {
            if (string.IsNullOrWhiteSpace(request.Address))
                return ResultModel.Invalid("Address is required");

            if (string.IsNullOrWhiteSpace(request.City))
                return ResultModel.Invalid("City is required");

            if (string.IsNullOrWhiteSpace(request.Country))
                return ResultModel.Invalid("Country is required");

            request.UpdatedBy = username;
            request.UpdatedAt = DateTime.UtcNow;

            if (request.DistanceId == null || request.DistanceId == 0)
            {
                request.CreatedBy = username;
                request.CreatedAt = DateTime.UtcNow;

                await _repo.AddAsync(request);
                return ResultModel.Created("Distance added successfully");
            }

            var updated = await _repo.UpdateAsync(request.DistanceId.Value, request);
            if (updated == null)
                return ResultModel.NotFound("Distance not found");

            return ResultModel.Updated("Distance updated successfully");
        }

        public async Task<DistanceResponse?> GetByIdAsync(int id)
            => await _repo.GetByIdAsync(id);

        public async Task<IEnumerable<DistanceResponse>> GetAllAsync()
            => await _repo.GetAllAsync();

        public async Task<IEnumerable<DistanceResponse>> GetByFilterAsync(DistanceFilterModel filter)
            => await _repo.GetByFilterAsync(filter);

        public async Task<bool> DeleteAsync(int id, string username)
            => await _repo.SoftDeleteAsync(id, username);
    }
}