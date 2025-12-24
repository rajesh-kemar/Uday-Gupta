using kemar.WHL.Model.Filter;
using Kemar.WHL.Business.Interfaces;
using Kemar.WHL.Model.Common;
using Kemar.WHL.Repository.Interfaces;
using Kemar.WHL.Repository.Models.User;

namespace Kemar.WHL.Business.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repo;

        public UserService(IUserRepository repo)
        {
            _repo = repo;
        }

        // ---------------------------------------------------------
        // ADD or UPDATE USER
        // ---------------------------------------------------------
        public async Task<ResultModel> AddOrUpdateAsync(UserRequest request, int loggedInUserId)
        {
            // Basic validations
            if (string.IsNullOrWhiteSpace(request.Username))
                return ResultModel.Invalid("Username is required");

            if (string.IsNullOrWhiteSpace(request.Email))
                return ResultModel.Invalid("Email is required");

            if (request.RoleIds == null || !request.RoleIds.Any())
                return ResultModel.Invalid("At least one role is required");

            if (request.UserId == null || request.UserId == 0)
            {
               // request.CreatedBy = loggedInUserId;

                var added = await _repo.AddAsync(request);

                return ResultModel.Created("User added successfully", added);
            }

            //request.UpdatedBy = loggedInUserId;

            var updated = await _repo.UpdateAsync(request.UserId.Value, request);

            if (updated == null)
                return ResultModel.NotFound("User not found");

            return ResultModel.Updated("User updated successfully", updated);
        }

        // ---------------------------------------------------------
        // GET ALL USERS
        // ---------------------------------------------------------
        public async Task<IEnumerable<UserResponse>> GetAllAsync()
            => await _repo.GetAllAsync();

        // ---------------------------------------------------------
        // FILTER USERS
        // ---------------------------------------------------------
        public async Task<IEnumerable<UserResponse>> GetByFilterAsync(UserFilterModel filter)
            => await _repo.GetByFilterAsync(filter);

        // ---------------------------------------------------------
        // GET USER BY ID
        // ---------------------------------------------------------
        public async Task<UserResponse?> GetByIdAsync(int id)
            => await _repo.GetByIdAsync(id);

        // ---------------------------------------------------------
        // DELETE USER
        // ---------------------------------------------------------
        public async Task<bool> DeleteAsync(int id, int loggedInUserId)
            => await _repo.DeleteAsync(id);
    }
}