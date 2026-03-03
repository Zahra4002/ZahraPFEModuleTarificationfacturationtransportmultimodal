using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Application/Interfaces/IJwtProvider.cs
using System.Security.Claims;
using Domain.Entities;

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
