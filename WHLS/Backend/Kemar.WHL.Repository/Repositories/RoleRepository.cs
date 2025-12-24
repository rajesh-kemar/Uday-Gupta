using Kemar.WHL.Repository.Context;
using Kemar.WHL.Repository.Entities;
using Kemar.WHL.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

public class RoleRepository : IRoleRepository
{
    private readonly WarehouseDbContext _context;
    public RoleRepository(WarehouseDbContext context) => _context = context;

    public async Task<Role?> GetByIdAsync(int id) =>
        await _context.Roles.FindAsync(id);

    public async Task<Role?> GetByNameAsync(string name) =>
        await _context.Roles
            .FirstOrDefaultAsync(r => r.Name.ToLower() == name.ToLower());

    public async Task<IEnumerable<Role>> GetAllAsync() =>
        await _context.Roles.ToListAsync();
}