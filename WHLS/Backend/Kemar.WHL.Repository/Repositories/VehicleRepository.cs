using AutoMapper;
using Kemar.WHL.Model.Request;
using Kemar.WHL.Model.Response;
using Kemar.WHL.Repository.Context;
using Kemar.WHL.Repository.Entities;
using Kemar.WHL.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Kemar.WHL.Repository.Repositories
{
    public class VehicleRepository : IVehicleRepository
    {
        private readonly WarehouseDbContext _context;
        private readonly IMapper _mapper;

        public VehicleRepository(WarehouseDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<VehicleResponse> AddAsync(VehicleRequest request)
        {
            var entity = _mapper.Map<Vehicle>(request);

            entity.CreatedBy = request.CreatedBy;
            entity.CreatedAt = request.CreatedAt;

            entity.UpdatedBy = request.UpdatedBy;
            entity.UpdatedAt = request.UpdatedAt;

            await _context.Vehicles.AddAsync(entity);
            await _context.SaveChangesAsync();

            return _mapper.Map<VehicleResponse>(entity);
        }

        public async Task<VehicleResponse?> UpdateAsync(int id, VehicleRequest request)
        {
            var entity = await _context.Vehicles.FirstOrDefaultAsync(x => x.VehicleId == id && !x.IsDeleted);
            if (entity == null) return null;

            _mapper.Map(request, entity);

            entity.UpdatedBy = request.UpdatedBy;
            entity.UpdatedAt = request.UpdatedAt;

            await _context.SaveChangesAsync();
            return _mapper.Map<VehicleResponse>(entity);
        }

        public async Task<IEnumerable<VehicleResponse>> GetAllAsync()
        {
            var entities = await _context.Vehicles.Where(x => !x.IsDeleted).ToListAsync();
            return _mapper.Map<IEnumerable<VehicleResponse>>(entities);
        }

        public async Task<IEnumerable<VehicleResponse>> GetByFilterAsync(VehicleFilterRequest filter)
        {
            var query = _context.Vehicles.AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.VehicleNumber))
                query = query.Where(x => x.VehicleNumber.Contains(filter.VehicleNumber));

            if (!string.IsNullOrWhiteSpace(filter.Type))
                query = query.Where(x => x.Type.Contains(filter.Type));

            if (filter.MinCapacity > 0)
                query = query.Where(x => x.Capacity >= filter.MinCapacity);

            if (filter.MaxCapacity > 0)
                query = query.Where(x => x.Capacity <= filter.MaxCapacity);

            query = query.Where(x => !x.IsDeleted);

            var entities = await query.ToListAsync();
            return _mapper.Map<IEnumerable<VehicleResponse>>(entities);
        }

        public async Task<VehicleResponse?> GetByIdAsync(int id)
        {
            var entity = await _context.Vehicles.FirstOrDefaultAsync(x => x.VehicleId == id && !x.IsDeleted);
            return entity == null ? null : _mapper.Map<VehicleResponse>(entity);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _context.Vehicles.FirstOrDefaultAsync(x => x.VehicleId == id && !x.IsDeleted);
            if (entity == null) return false;

            entity.IsDeleted = true;
            entity.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }
    }

}