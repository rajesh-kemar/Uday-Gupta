using AutoMapper;
using Kemar.WHL.Model.Request;
using Kemar.WHL.Model.Response;
using Kemar.WHL.Repository.Context;
using Kemar.WHL.Repository.Entities;
using Microsoft.EntityFrameworkCore;

namespace Kemar.WHL.Repository.Repositories
{
    public class InventoryRepository : IInventoryRepository
    {
        private readonly WarehouseDbContext _context;
        private readonly IMapper _mapper;

        public InventoryRepository(WarehouseDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // ===================== CRUD =====================

        public async Task<InventoryResponse> AddAsync(InventoryRequest request)
        {
            var entity = _mapper.Map<Inventory>(request);
            await _context.Inventories.AddAsync(entity);
            await _context.SaveChangesAsync();
            return _mapper.Map<InventoryResponse>(entity);
        }

        public async Task<InventoryResponse?> UpdateAsync(int id, InventoryRequest request)
        {
            var entity = await _context.Inventories
                .FirstOrDefaultAsync(x => x.InventoryId == id && !x.IsDeleted);

            if (entity == null) return null;

            _mapper.Map(request, entity);
            entity.UpdatedBy = request.UpdatedBy;
            entity.UpdatedAt = request.UpdatedAt;

            await _context.SaveChangesAsync();
            return _mapper.Map<InventoryResponse>(entity);
        }

        public async Task<IEnumerable<InventoryResponse>> GetAllAsync()
        {
            var list = await _context.Inventories
                .Include(x => x.Warehouse)
                .Include(x => x.Product)
                .Where(x => !x.IsDeleted)
                .ToListAsync();

            return _mapper.Map<IEnumerable<InventoryResponse>>(list);
        }

        public async Task<IEnumerable<InventoryResponse>> GetByFilterAsync(InventoryFilterModel filter)
        {
            var query = _context.Inventories
                .Include(x => x.Warehouse)
                .Include(x => x.Product)
                .Where(x => !x.IsDeleted)
                .AsQueryable();

            if (filter.WarehouseId.HasValue)
                query = query.Where(x => x.WarehouseId == filter.WarehouseId);

            if (filter.ProductId.HasValue)
                query = query.Where(x => x.ProductId == filter.ProductId);

            if (filter.MinQuantity.HasValue)
                query = query.Where(x => x.Quantity >= filter.MinQuantity);

            if (filter.MaxQuantity.HasValue)
                query = query.Where(x => x.Quantity <= filter.MaxQuantity);

            var result = await query.ToListAsync();
            return _mapper.Map<IEnumerable<InventoryResponse>>(result);
        }

        public async Task<InventoryResponse?> GetByIdAsync(int id)
        {
            var entity = await _context.Inventories
                .Include(x => x.Warehouse)
                .Include(x => x.Product)
                .FirstOrDefaultAsync(x => x.InventoryId == id && !x.IsDeleted);

            return entity == null ? null : _mapper.Map<InventoryResponse>(entity);
        }

        public async Task<bool> SoftDeleteAsync(int id, string username)
        {
            var entity = await _context.Inventories
                .FirstOrDefaultAsync(x => x.InventoryId == id && !x.IsDeleted);

            if (entity == null) return false;

            entity.IsDeleted = true;
            entity.UpdatedBy = username;
            entity.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        // ===================== PICKING SUPPORT =====================

        public async Task<Inventory?> GetByWarehouseAndProductAsync(int warehouseId, int productId)
        {
            return await _context.Inventories
                .FirstOrDefaultAsync(x =>
                    x.WarehouseId == warehouseId &&
                    x.ProductId == productId &&
                    !x.IsDeleted);
        }

        public async Task UpdateEntityAsync(Inventory inventory)
        {
            _context.Inventories.Update(inventory);
            await _context.SaveChangesAsync();
        }
    }
}
