using Kemar.WHL.Business.Interfaces;
using Kemar.WHL.Repository.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

public class SlidingExpirationMiddleware
{
    private readonly RequestDelegate _next;

    public SlidingExpirationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        string? header = context.Request.Headers["Authorization"];

        if (!string.IsNullOrWhiteSpace(header) && header.StartsWith("Bearer "))
        {
            string token = header.Substring("Bearer ".Length);

            // CREATE SCOPE FOR SCOPED SERVICES
            using var scope = context.RequestServices.CreateScope();

            var tokenService = scope.ServiceProvider.GetRequiredService<ITokenService>();
            var userRepo = scope.ServiceProvider.GetRequiredService<IUserRepository>();

            var principal = tokenService.ValidateToken(token);

            if (principal != null)
            {
                string? userIdStr = principal.FindFirstValue(ClaimTypes.NameIdentifier);

                if (int.TryParse(userIdStr, out int userId))
                {
                    var user = await userRepo.GetEntityByIdAsync(userId);

                    if (user != null)
                    {
                        // Read JWT token data
                        var handler = new JwtSecurityTokenHandler();
                        var jwt = handler.ReadJwtToken(token);

                        // How long token is left
                        var remaining = jwt.ValidTo - DateTime.UtcNow;

                        // Update last activity always
                        await userRepo.UpdateLastActivityAsync(userId, DateTime.UtcNow);

                        // ***** SLIDING TOKEN REFRESH *****
                        if (remaining < TimeSpan.FromMinutes(2))
                        {
                            var roles = principal.Claims
                                    .Where(c => c.Type == ClaimTypes.Role)
                                    .Select(c => c.Value)
                                    .ToList();

                            string newToken = tokenService.GenerateToken(
                                userId,
                                principal.Identity!.Name!,
                                roles
                            );

                            context.Response.Headers["X-New-Token"] = newToken;
                        }
                    }
                }
            }
        }

        await _next(context);
    }
}