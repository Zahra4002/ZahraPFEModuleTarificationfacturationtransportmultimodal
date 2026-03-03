using Application.Features.AuthFeature.Dtos;
using Application.Interfaces;
using Application.Setting;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Application.Features.AuthFeature.Commands
{
    public record RefreshTokenCommand(RefreshTokenRequestDTO RefreshRequest) : IRequest<ResponseHttp>;

    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, ResponseHttp>
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtProvider _jwtProvider;

        public RefreshTokenCommandHandler(IUserRepository userRepository, IJwtProvider jwtProvider)
        {
            _userRepository = userRepository;
            _jwtProvider = jwtProvider;
        }

        public async Task<ResponseHttp> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Get principal from expired token
                var principal = _jwtProvider.GetPrincipalFromExpiredToken(request.RefreshRequest.RefreshToken);
                if (principal == null)
                {
                    return new ResponseHttp
                    {
                        Fail_Messages = "Invalid token",
                        Status = StatusCodes.Status401Unauthorized
                    };
                }

                // Get user ID from claims
                var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
                            principal.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ??
                            principal.FindFirst("sub")?.Value;

                if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var userGuid))
                {
                    return new ResponseHttp
                    {
                        Fail_Messages = "Invalid token claims",
                        Status = StatusCodes.Status401Unauthorized
                    };
                }

                // Get user from database
                var user = await _userRepository.GetByIdAsync(userGuid, cancellationToken);
                if (user == null || user.RefreshToken != request.RefreshRequest.RefreshToken ||
                    user.RefreshTokenExpiryTime <= DateTime.UtcNow)
                {
                    return new ResponseHttp
                    {
                        Fail_Messages = "Invalid refresh token",
                        Status = StatusCodes.Status401Unauthorized
                    };
                }

                // Generate new tokens
                var newToken = _jwtProvider.GenerateToken(user);
                var newRefreshToken = _jwtProvider.GenerateRefreshToken();

                // Update user with new refresh token
                user.RefreshToken = newRefreshToken;
                user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
                await _userRepository.Update(user);
                await _userRepository.SaveChange(cancellationToken);

                // Prepare response
                var response = new AuthResponseDTO
                {
                    Token = newToken,
                    RefreshToken = newRefreshToken,
                    Expiry = DateTime.UtcNow.AddMinutes(60),
                    User = new UserDTO
                    {
                        Id = user.Id,
                        Email = user.Email,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        PhoneNumber = user.PhoneNumber,
                        Role = user.Role.ToString(),
                        IsActive = user.IsActive,
                        LastLoginAt = user.LastLoginAt
                    }
                };

                return new ResponseHttp
                {
                    Resultat = response,
                    Status = StatusCodes.Status200OK
                };
            }
            catch (Exception ex)
            {
                return new ResponseHttp
                {
                    Fail_Messages = ex.Message,
                    Status = StatusCodes.Status400BadRequest
                };
            }
        }
    }
}