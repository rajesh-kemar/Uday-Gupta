using AdvLogisticSystem.Data;
using AdvLogisticSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace AdvLogisticSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly LodisticsDbContext _context;

        public AuthController(IConfiguration config, LodisticsDbContext context)
        {
            _config = config;
            _context = context;
        }

        // ✅ Simple test endpoint
        [HttpGet]
        public IActionResult GetInfo()
        {
            return Ok(new
            {
                status = "ok",
                message = "Auth API is running successfully!"
            });
        }

        // ✅ Get logged-in user info
        [HttpGet("me")]
        [Authorize]
        public IActionResult GetUserInfo()
        {
            var username = User.Identity?.Name ?? "Unknown";
            var role = User.FindFirst(ClaimTypes.Role)?.Value ?? "No Role";
            return Ok(new { Username = username, Role = role });
        }

        // ✅ Login endpoint
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] JsonElement payload)
        {
            string username = payload.GetProperty("username").GetString() ?? "";
            string password = payload.GetProperty("password").GetString() ?? "";

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                return BadRequest(new { message = "Username and password are required." });

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username && u.Password == password);

            if (user == null)
                return Unauthorized(new { message = "Invalid username or password" });

            // ✅ Generate JWT Token
            var token = GenerateJwtToken(user.Username, user.Role);

            // ✅ If Driver, fetch driver record
            int? driverId = null;
            if (user.Role == "Driver")
            {
                var driver = await _context.Drivers
                    .FirstOrDefaultAsync(d => d.Name == user.Username || d.Phone == user.Username);

                driverId = driver?.Id;
            }

            // ✅ Return token, role, and driverId to frontend
            return Ok(new
            {
                token,
                role = user.Role,
                driverId
            });
        }

        // ✅ Register endpoint
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] JsonElement payload)
        {
            string username = payload.GetProperty("username").GetString() ?? "";
            string password = payload.GetProperty("password").GetString() ?? "";
            string role = payload.TryGetProperty("role", out var roleProp)
                ? roleProp.GetString() ?? "Driver"
                : "Driver";

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                return BadRequest(new { message = "Username and password are required." });

            if (await _context.Users.AnyAsync(u => u.Username == username))
                return BadRequest(new { message = "Username already exists." });

            int? driverId = null;

            // ✅ Create Driver record if role = Driver
            if (role == "Driver")
            {
                string name = "";
                if (payload.TryGetProperty("name", out var nameProp))
                    name = nameProp.GetString() ?? "";
                else
                {
                    string firstName = payload.TryGetProperty("firstName", out var fn) ? fn.GetString() ?? "" : "";
                    string lastName = payload.TryGetProperty("lastName", out var ln) ? ln.GetString() ?? "" : "";
                    name = $"{firstName} {lastName}".Trim();
                }

                string phone = payload.TryGetProperty("phoneNumber", out var ph) ? ph.GetString() ?? "" :
                               payload.TryGetProperty("phone", out var ph2) ? ph2.GetString() ?? "" : "";

                string licenseNumber = payload.TryGetProperty("licenseNumber", out var lic) ? lic.GetString() ?? "" : "";

                int experience = payload.TryGetProperty("experience", out var exp) && exp.ValueKind == JsonValueKind.Number
                    ? exp.GetInt32()
                    : 0;

                var driver = new Driver
                {
                    Name = name,
                    LicenseNumber = licenseNumber,
                    Phone = phone,
                    Experience = experience,
                    IsAvailable = true
                };

                _context.Drivers.Add(driver);
                await _context.SaveChangesAsync();
                driverId = driver.Id;
            }

            // ✅ Save the user record
            var user = new User
            {
                Username = username,
                Password = password, // ⚠️ In production, always hash passwords!
                Role = role
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Registration successful",
                userId = user.Id,
                driverId
            });
        }

        // ✅ Helper to generate JWT token
        private string GenerateJwtToken(string username, string role)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, role)
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Issuer"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
