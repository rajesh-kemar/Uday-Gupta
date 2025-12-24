using AutoMapper;
using Kemar.WHL.Model.Request;
using Kemar.WHL.Model.Response;
using Kemar.WHL.Repository.Context;
using Kemar.WHL.Repository.Entities;
using Kemar.WHL.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kemar.WHL.Repository.Repositories
{
    public class DistanceRepository : IDistanceRepository
    {
        private readonly WarehouseDbContext _context;
        private readonly IMapper _mapper;

        public DistanceRepository(WarehouseDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<DistanceResponse> AddAsync(DistanceRequest request)
        {
            var entity = _mapper.Map<Distance>(request);
            await _context.Distances.AddAsync(entity);
            await _context.SaveChangesAsync();
            return _mapper.Map<DistanceResponse>(entity);
        }

        public async Task<DistanceResponse?> UpdateAsync(int id, DistanceRequest request)
        {
            var entity = await _context.Distances
                .FirstOrDefaultAsync(x => x.DistanceId == id && !x.IsDeleted);

            if (entity == null) return null;

            _mapper.Map(request, entity);
            entity.UpdatedAt = DateTime.UtcNow;
            entity.UpdatedBy = request.UpdatedBy;

            await _context.SaveChangesAsync();
            return _mapper.Map<DistanceResponse>(entity);
        }

        public async Task<IEnumerable<DistanceResponse>> GetAllAsync()
        {
            var list = await _context.Distances
                .Where(x => !x.IsDeleted)
                .ToListAsync();

            return _mapper.Map<IEnumerable<DistanceResponse>>(list);
        }

        public async Task<IEnumerable<DistanceResponse>> GetByFilterAsync(DistanceFilterModel filter)
        {
            var query = _context.Distances
                .Where(x => !x.IsDeleted)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.Address))
                query = query.Where(x => x.Address.Contains(filter.Address));

            if (!string.IsNullOrWhiteSpace(filter.City))
                query = query.Where(x => x.City.Contains(filter.City));

            if (!string.IsNullOrWhiteSpace(filter.Country))
                query = query.Where(x => x.Country.Contains(filter.Country));

            var entities = await query.OrderBy(x => x.DistanceId).ToListAsync();
            return _mapper.Map<IEnumerable<DistanceResponse>>(entities);
        }

        public async Task<DistanceResponse?> GetByIdAsync(int id)
        {
            var entity = await _context.Distances
                .FirstOrDefaultAsync(x => x.DistanceId == id && !x.IsDeleted);

            return entity == null ? null : _mapper.Map<DistanceResponse>(entity);
        }

        public async Task<bool> SoftDeleteAsync(int id, string username)
        {
            var entity = await _context.Distances
                .FirstOrDefaultAsync(x => x.DistanceId == id && !x.IsDeleted);

            if (entity == null) return false;

            entity.IsDeleted = true;
            entity.UpdatedAt = DateTime.UtcNow;
            entity.UpdatedBy = username;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}