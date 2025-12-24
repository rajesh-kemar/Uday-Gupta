using AutoMapper;
using Kemar.WHL.Model.Request;
using Kemar.WHL.Model.Response;
using Kemar.WHL.Repository.Context;
using Kemar.WHL.Repository.Entities;
using Microsoft.EntityFrameworkCore;

public class ProductRepository : IProductRepository
{
    private readonly WarehouseDbContext _context;
    private readonly IMapper _mapper;

    public ProductRepository(WarehouseDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ProductResponse> AddAsync(ProductRequest request)
    {
        var entity = _mapper.Map<Product>(request);
        await _context.Products.AddAsync(entity);
        await _context.SaveChangesAsync();
        return _mapper.Map<ProductResponse>(entity);
    }

    public async Task<ProductResponse?> UpdateAsync(int id, ProductRequest request)
    {
        var entity = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == id && !p.IsDeleted);
        if (entity == null) return null;

        _mapper.Map(request, entity);
        await _context.SaveChangesAsync();
        return _mapper.Map<ProductResponse>(entity);
    }

    public async Task<IEnumerable<ProductResponse>> GetAllAsync()
    {
        var list = await _context.Products.Where(x => !x.IsDeleted).ToListAsync();
        return _mapper.Map<IEnumerable<ProductResponse>>(list);
    }

    public async Task<ProductResponse?> GetByIdAsync(int id)
    {
        var entity = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == id && !p.IsDeleted);
        return entity == null ? null : _mapper.Map<ProductResponse>(entity);
    }

    public async Task<IEnumerable<ProductResponse>> GetByFilterAsync(ProductFilterRequest filter)
    {
        var query = _context.Products.AsQueryable().Where(x => !x.IsDeleted);

        if (!string.IsNullOrEmpty(filter.Name))
            query = query.Where(p => p.Name.Contains(filter.Name));

        if (filter.MinWeight.HasValue)
            query = query.Where(p => p.Weight >= filter.MinWeight.Value);

        if (filter.MaxWeight.HasValue)
            query = query.Where(p => p.Weight <= filter.MaxWeight.Value);

        var list = await query.ToListAsync();
        return _mapper.Map<IEnumerable<ProductResponse>>(list);
    }

    public async Task<bool> SoftDeleteAsync(int id, string username)
    {
        var entity = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == id && !p.IsDeleted);
        if (entity == null) return false;

        entity.IsDeleted = true;
        entity.UpdatedBy = username;
        entity.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }
}