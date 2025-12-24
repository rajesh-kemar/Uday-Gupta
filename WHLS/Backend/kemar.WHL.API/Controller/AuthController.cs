using Kemar.WHL.Business.Interfaces;
using Kemar.WHL.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Kemar.WHL.Model.Request;

namespace Kemar.WHL.API.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _userRepo;
        private readonly ITokenService _tokenService;

        public AuthController(IUserRepository userRepo, ITokenService tokenService)
        {
            _userRepo = userRepo;
            _tokenService = tokenService;
        }

        public class LoginRequest
        {
            public string Email { get; set; } = "";
            public string Password { get; set; } = "";
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest req)
        {

            if (string.IsNullOrWhiteSpace(req.Email) || string.IsNullOrWhiteSpace(req.Password))
                return BadRequest("Email and password are required.");

            var userEntity = await _userRepo.GetEntityByEmailAsync(req.Email);
            if (userEntity == null)
                return Unauthorized("Invalid credentials");

            using var sha = SHA256.Create();
            var incomingPwdBytes = Encoding.UTF8.GetBytes(req.Password);
            var incomingHash = sha.ComputeHash(incomingPwdBytes);

            if (userEntity.PasswordHash == null || userEntity.PasswordHash.Length == 0)
                return Unauthorized("Password not set for this user.");

            if (!incomingHash.SequenceEqual(userEntity.PasswordHash))
                return Unauthorized("Invalid credentials");

            var roleNames = await _userRepo.GetUserRoleNamesAsync(userEntity.UserId);

            if (!roleNames.Any())
                return Unauthorized("No roles assigned to this user.");

            var token = _tokenService.GenerateToken(
                userEntity.UserId,
                userEntity.Username,
                roleNames.ToList()
            );

            return Ok(new
            {
                token,
                userId = userEntity.UserId,
                username = userEntity.Username,
                roles = roleNames
            });
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.OldPassword) ||
                string.IsNullOrWhiteSpace(req.NewPassword) ||
                string.IsNullOrWhiteSpace(req.ConfirmPassword))
                return BadRequest("All fields are required.");

            if (req.NewPassword != req.ConfirmPassword)
                return BadRequest("New password and confirm password do not match.");

            // Get logged-in user id from token
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null)
                return Unauthorized("Invalid user token.");

            int userId = int.Parse(userIdClaim);

            // Fetch user
            var userEntity = await _userRepo.GetEntityByIdAsync(userId);
            if (userEntity == null)
                return Unauthorized("User not found.");

            using var sha = SHA256.Create();
            // Compare old password
            var oldHash = sha.ComputeHash(Encoding.UTF8.GetBytes(req.OldPassword));

            if (!oldHash.SequenceEqual(userEntity.PasswordHash))
                return BadRequest("Old password is wrong.");

            // Hash new password
            var newHash = sha.ComputeHash(Encoding.UTF8.GetBytes(req.NewPassword));

            // Update in DB
            userEntity.PasswordHash = newHash;
            await _userRepo.UpdatePasswordAsync(userEntity);

            return Ok("Password changed successfully.");
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.Email) ||
                string.IsNullOrWhiteSpace(req.NewPassword) ||
                string.IsNullOrWhiteSpace(req.ConfirmPassword))
                return BadRequest("All fields are required.");

            if (req.NewPassword != req.ConfirmPassword)
                return BadRequest("New password and confirm password do not match.");

            // Get user by email
            var userEntity = await _userRepo.GetEntityByEmailAsync(req.Email);
            if (userEntity == null)
                return BadRequest("User with this email does not exist.");

            using var sha = SHA256.Create();
            var newHash = sha.ComputeHash(Encoding.UTF8.GetBytes(req.NewPassword));

            userEntity.PasswordHash = newHash;
            await _userRepo.UpdatePasswordAsync(userEntity);

            return Ok("Password reset successfully. You can now log in with new password.");
        }
    }
}