using AutoMapper;
using Kemar.WHL.Model.Request;
using Kemar.WHL.Model.Response;
using Kemar.WHL.Repository.Context;
using Kemar.WHL.Repository.Entities;
using Kemar.WHL.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Kemar.WHL.Repository.Repositories
{
    public class WarehouseRepository : IWarehouseRepository
    {
        private readonly WarehouseDbContext _context;
        private readonly IMapper _mapper;

        public WarehouseRepository(WarehouseDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }       

        public async Task<WarehouseResponse> AddAsync(WarehouseRequest request)
        {
            var entity = _mapper.Map<Warehouse>(request);

            // Set audit fields
            entity.CreatedBy = request.CreatedBy;
            entity.CreatedAt = request.CreatedAt;
            entity.UpdatedBy = request.UpdatedBy;
            entity.UpdatedAt = request.UpdatedAt;

            await _context.Warehouses.AddAsync(entity);
            await _context.SaveChangesAsync();

            return _mapper.Map<WarehouseResponse>(entity);
        }

        public async Task<WarehouseResponse?> UpdateAsync(int id, WarehouseRequest request)
        {
            var entity = await _context.Warehouses
                .FirstOrDefaultAsync(x => x.WarehouseId == id && !x.IsDeleted);

            if (entity == null) return null;

            // Map updated fields
            _mapper.Map(request, entity);

            // Preserve CreatedBy/CreatedAt, update UpdatedBy/UpdatedAt
            entity.UpdatedBy = request.UpdatedBy;
            entity.UpdatedAt = request.UpdatedAt;

            await _context.SaveChangesAsync();

            return _mapper.Map<WarehouseResponse>(entity);
        }

        public async Task<IEnumerable<WarehouseResponse>> GetAllAsync()
        {
            var entities = await _context.Warehouses
                .Where(x => !x.IsDeleted)
                .ToListAsync();

            return _mapper.Map<IEnumerable<WarehouseResponse>>(entities);
        }

        public async Task<IEnumerable<WarehouseResponse>> GetByFilterAsync(WarehouseFilterModel filter)
        {
            var query = _context.Warehouses.AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.Name))
                query = query.Where(x => x.Name.Contains(filter.Name));

            if (!string.IsNullOrWhiteSpace(filter.Location))
                query = query.Where(x => x.Location.Contains(filter.Location));

            if (filter.MinCapacity > 0)
                query = query.Where(x => x.Capacity >= filter.MinCapacity);

            if (filter.MaxCapacity > 0)
                query = query.Where(x => x.Capacity <= filter.MaxCapacity);

            query = query.Where(x => !x.IsDeleted);

            var entities = await query.ToListAsync();
            return _mapper.Map<IEnumerable<WarehouseResponse>>(entities);
        }

        public async Task<WarehouseResponse?> GetByIdAsync(int id)
        {
            var entity = await _context.Warehouses
                .FirstOrDefaultAsync(x => x.WarehouseId == id && !x.IsDeleted);

            return entity == null ? null : _mapper.Map<WarehouseResponse>(entity);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _context.Warehouses
                .FirstOrDefaultAsync(x => x.WarehouseId == id && !x.IsDeleted);

            if (entity == null) return false;

            entity.IsDeleted = true;
            entity.UpdatedAt = DateTime.UtcNow; 
            await _context.SaveChangesAsync();

            return true;
        }
    }
}