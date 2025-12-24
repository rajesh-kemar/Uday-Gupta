using System.Security.Claims;

namespace Kemar.WHL.Business.Interfaces
{
    public interface ITokenService
    {
        string GenerateToken(int userId, string username, List<string> roles);
        ClaimsPrincipal? ValidateToken(string token);
    }
}