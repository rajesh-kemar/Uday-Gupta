using AutoMapper;
using Kemar.WHL.Model.Request;
using Kemar.WHL.Model.Response;
using Kemar.WHL.Repository.Context;
using Kemar.WHL.Repository.Entities;
using Microsoft.EntityFrameworkCore;

public class PickingRepository : IPickingRepository
{
    private readonly WarehouseDbContext _context;
    private readonly IMapper _mapper;

    public PickingRepository(WarehouseDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PickingResponse> AddAsync(PickingRequest request)
    {
        var entity = _mapper.Map<Picking>(request);

        await _context.Pickings.AddAsync(entity);
        await _context.SaveChangesAsync();

        return _mapper.Map<PickingResponse>(entity);
    }

    public async Task<List<PickingResponse>> GetByShipmentAsync(int shipmentId)
    {
        var list = await _context.Pickings
            .Include(x => x.Product)
            .Include(x => x.Warehouse)
            .Where(x => x.ShipmentId == shipmentId)
            .ToListAsync();

        return _mapper.Map<List<PickingResponse>>(list);
    }
}
