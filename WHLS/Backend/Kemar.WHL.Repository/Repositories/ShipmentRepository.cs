using AutoMapper;
using kemar.WHL.Model.Filter;
using Kemar.WHL.Model.Response;
using Kemar.WHL.Repository.Context;
using Microsoft.EntityFrameworkCore;

public class ShipmentRepository : IShipmentRepository
{
    private readonly WarehouseDbContext _context;
    private readonly IMapper _mapper;

    public ShipmentRepository(WarehouseDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Shipment> AddAsync(Shipment entity)
    {
        await _context.Shipments.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<Shipment?> UpdateAsync(Shipment entity)
    {
        // Entity is already tracked — just save
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<bool> DeleteAsync(int id, string username)
    {
        var entity = await _context.Shipments
            .FirstOrDefaultAsync(x => x.ShipmentId == id && !x.IsDeleted);

        if (entity == null) return false;

        entity.IsDeleted = true;
        entity.UpdatedBy = username;
        entity.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    // 🔴 IMPORTANT: This returns ENTITY (used for update)
    public async Task<Shipment?> GetEntityByIdAsync(int id)
    {
        return await _context.Shipments
            .FirstOrDefaultAsync(x => x.ShipmentId == id && !x.IsDeleted);
    }

    public async Task<ShipmentResponse?> GetByIdAsync(int id)
    {
        var entity = await _context.Shipments
            .Include(x => x.Vehicle)
            .Include(x => x.Destination)
            .FirstOrDefaultAsync(x => x.ShipmentId == id && !x.IsDeleted);

        return entity == null ? null : _mapper.Map<ShipmentResponse>(entity);
    }

    public async Task<List<ShipmentResponse>> GetByFilterAsync(ShipmentFilterModel filter)
    {
        var query = _context.Shipments
            .Include(x => x.Vehicle)
            .Include(x => x.Destination)
            .Where(x => !x.IsDeleted)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.ShipmentNumber))
            query = query.Where(x => x.ShipmentNumber.Contains(filter.ShipmentNumber));

        if (filter.DestinationId.HasValue)
            query = query.Where(x => x.DestinationId == filter.DestinationId);

        if (filter.VehicleId.HasValue)
            query = query.Where(x => x.VehicleId == filter.VehicleId);

        return _mapper.Map<List<ShipmentResponse>>(await query.ToListAsync());
    }

    public async Task<List<ShipmentResponse>> GetAllAsync()
    {
        var list = await _context.Shipments
            .Include(x => x.Vehicle)
            .Include(x => x.Destination)
            .Where(x => !x.IsDeleted)
            .ToListAsync();

        return _mapper.Map<List<ShipmentResponse>>(list);
    }
}
