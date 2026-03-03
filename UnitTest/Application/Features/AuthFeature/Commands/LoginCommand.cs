// Application/Features/AuthFeature/Commands/LoginCommand.cs
using Application.Features.AuthFeature.Dtos;
using Application.Interfaces;
using Application.Setting;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using BCrypt.Net;

namespace Application.Features.AuthFeature.Commands
{
    public record LoginCommand(LoginRequestDTO LoginRequest) : IRequest<ResponseHttp>;

    public class LoginCommandHandler : IRequestHandler<LoginCommand, ResponseHttp>
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtProvider _jwtProvider;

        public LoginCommandHandler(IUserRepository userRepository, IJwtProvider jwtProvider)
        {
            _userRepository = userRepository;
            _jwtProvider = jwtProvider;
        }

        public async Task<ResponseHttp> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Find user by email
                var user = await _userRepository.GetByEmailAsync(request.LoginRequest.Email);

                // User not found
                if (user == null)
                {
                    return new ResponseHttp
                    {
                        Fail_Messages = "User not found",
                        Status = StatusCodes.Status401Unauthorized
                    };
                }

                // Check if user is locked out
                if (user.IsLockedOut)
                {
                    return new ResponseHttp
                    {
                        Fail_Messages = "Account is locked out. Please try again later.",
                        Status = StatusCodes.Status401Unauthorized
                    };
                }

                // Check if user is active
                if (!user.IsActive)
                {
                    return new ResponseHttp
                    {
                        Fail_Messages = "Account is deactivated",
                        Status = StatusCodes.Status401Unauthorized
                    };
                }

                // Verify password
                if (!BCrypt.Net.BCrypt.Verify(request.LoginRequest.Password, user.PasswordHash))
                {
                    // Record failed login attempt
                    user.RecordLoginFailure();
                    await _userRepository.Update(user);
                    await _userRepository.SaveChange(cancellationToken);

                    return new ResponseHttp
                    {
                        Fail_Messages = "Invalid credentials",
                        Status = StatusCodes.Status401Unauthorized
                    };
                }

                // Password is correct - record successful login
                user.RecordLoginSuccess();

                // Generate refresh token
                user.RefreshToken = _jwtProvider.GenerateRefreshToken();
                user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

                await _userRepository.Update(user);
                await _userRepository.SaveChange(cancellationToken);

                // Generate JWT token
                var token = _jwtProvider.GenerateToken(user);

                // Prepare response
                var response = new AuthResponseDTO
                {
                    Token = token,
                    RefreshToken = user.RefreshToken,
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