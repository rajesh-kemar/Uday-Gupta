using AutoMapper;
using kemar.WHL.Model.Filter;
using Kemar.WHL.Repository.Context;
using Kemar.WHL.Repository.Entities;
using Kemar.WHL.Repository.Interfaces;
using Kemar.WHL.Repository.Models.User;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace Kemar.WHL.Repository.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly WarehouseDbContext _context;
        private readonly IMapper _mapper;

        public UserRepository(WarehouseDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        private byte[] HashPassword(string password)
        {
            using var sha = SHA256.Create();
            return sha.ComputeHash(Encoding.UTF8.GetBytes(password));
        }

        public async Task<UserResponse> AddAsync(UserRequest request)
        {
            var entity = _mapper.Map<User>(request);
            entity.PasswordHash = HashPassword(request.Password);
            entity.CreatedAt = DateTime.UtcNow;

            await _context.Users.AddAsync(entity);
            await _context.SaveChangesAsync();

            if (request.RoleIds != null)
            {
                foreach (var r in request.RoleIds)
                {
                    await _context.UserRoles.AddAsync(new UserRole
                    {
                        UserId = entity.UserId,
                        RoleId = r
                    });
                }
                await _context.SaveChangesAsync();
            }

            return _mapper.Map<UserResponse>(entity);
        }

        public async Task<UserResponse?> UpdateAsync(int id, UserRequest request)
        {
            var entity = await _context.Users
                .Include(u => u.UserRoles)
                .FirstOrDefaultAsync(u => u.UserId == id && !u.IsDeleted);

            if (entity == null)
                return null;

            entity.Username = request.Username;
            entity.Email = request.Email;
            entity.UpdatedBy = request.UpdatedBy;
            entity.UpdatedAt = DateTime.UtcNow;

            if (!string.IsNullOrWhiteSpace(request.Password))
                entity.PasswordHash = HashPassword(request.Password);

            entity.UserRoles.Clear();

            if (request.RoleIds != null)
            {
                foreach (var r in request.RoleIds)
                {
                    entity.UserRoles.Add(new UserRole
                    {
                        UserId = entity.UserId,
                        RoleId = r
                    });
                }
            }

            await _context.SaveChangesAsync();
            return _mapper.Map<UserResponse>(entity);
        }

        public async Task<IEnumerable<UserResponse>> GetAllAsync()
        {
            var users = await _context.Users.Where(x => !x.IsDeleted).ToListAsync();
            return _mapper.Map<IEnumerable<UserResponse>>(users);
        }

        public async Task<IEnumerable<UserResponse>> GetByFilterAsync(UserFilterModel filter)
        {
            var query = _context.Users.Where(x => !x.IsDeleted).AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.Username))
                query = query.Where(x => x.Username.Contains(filter.Username));

            if (!string.IsNullOrWhiteSpace(filter.Email))
                query = query.Where(x => x.Email.Contains(filter.Email));

            return _mapper.Map<IEnumerable<UserResponse>>(await query.ToListAsync());
        }

        public async Task<UserResponse?> GetByIdAsync(int id)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.UserId == id && !x.IsDeleted);

            return user == null ? null : _mapper.Map<UserResponse>(user);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _context.Users
                .FirstOrDefaultAsync(x => x.UserId == id && !x.IsDeleted);

            if (entity == null) return false;

            entity.IsDeleted = true;
            entity.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<string>> GetUserRoleNamesAsync(int userId)
        {
            return await _context.UserRoles
                .Where(ur => ur.UserId == userId)
                .Include(ur => ur.Role)
                .Select(ur => ur.Role!.Name)
                .ToListAsync();
        }

        public async Task<User?> GetEntityByEmailAsync(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email && !u.IsDeleted);
        }

        public async Task<User?> GetEntityByIdAsync(int id)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.UserId == id && !x.IsDeleted);
        }

        public async Task UpdatePasswordAsync(User entity)
        {
            _context.Users.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateLastActivityAsync(int userId, DateTime time)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserId == userId);
            if (user != null)
            {
                user.LastActivityTime = time;
                await _context.SaveChangesAsync();
            }
        }

        // ✅ Implement GetByUsernameAsync
        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username && !u.IsDeleted);
        }
    }
}
