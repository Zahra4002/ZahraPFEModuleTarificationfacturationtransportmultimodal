using Domain.Entities;
using System.Security.Claims;

namespace Application.Interfaces
{
    public interface IJwtProvider
    {
        string GenerateToken(User user);
        string GenerateRefreshToken();
        ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
        bool ValidateToken(string token);
    }
}
