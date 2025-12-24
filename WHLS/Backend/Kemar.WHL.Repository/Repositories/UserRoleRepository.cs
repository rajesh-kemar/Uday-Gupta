using Kemar.WHL.Repository.Context;
using Kemar.WHL.Repository.Entities;
using Kemar.WHL.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Kemar.WHL.Repository.Repositories
{
    public class UserRoleRepository : IUserRoleRepository
    {
        private readonly WarehouseDbContext _context;

        public UserRoleRepository(WarehouseDbContext context)
        {
            _context = context;
        }

        public async Task AssignRolesAsync(int userId, IEnumerable<int> roleIds)
        {
            // Remove old roles
            var existing = await _context.UserRoles
                .Where(ur => ur.UserId == userId)
                .ToListAsync();

            _context.UserRoles.RemoveRange(existing);

            // Add new roles
            var newRoles = roleIds.Select(rid => new UserRole
            {
                UserId = userId,
                RoleId = rid
            });

            await _context.UserRoles.AddRangeAsync(newRoles);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<string>> GetUserRoleNamesAsync(int userId)
        {
            return await _context.UserRoles
                .Where(ur => ur.UserId == userId)
                .Include(ur => ur.Role)
                .Select(ur => ur.Role.Name)
                .ToListAsync();
        }
    }
}