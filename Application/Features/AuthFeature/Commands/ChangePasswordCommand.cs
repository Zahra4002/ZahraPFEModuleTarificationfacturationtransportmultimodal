// Application/Features/AuthFeature/Commands/ChangePasswordCommand.cs
using Application.Interfaces;
using Application.Setting;
using MediatR;
using Microsoft.AspNetCore.Http;
using BCrypt.Net;

namespace Application.Features.AuthFeature.Commands
{
    public record ChangePasswordCommand(Guid UserId, string CurrentPassword, string NewPassword) : IRequest<ResponseHttp>;

    public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, ResponseHttp>
    {
        private readonly IUserRepository _userRepository;

        public ChangePasswordCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<ResponseHttp> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
                if (user == null)
                {
                    return new ResponseHttp
                    {
                        Fail_Messages = "User not found",
                        Status = StatusCodes.Status404NotFound
                    };
                }

                // Verify current password
                if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.PasswordHash))
                {
                    return new ResponseHttp
                    {
                        Fail_Messages = "Current password is incorrect",
                        Status = StatusCodes.Status400BadRequest
                    };
                }

                // Update password
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
                user.UpdatedAt = DateTime.UtcNow;

                // Invalidate refresh tokens for security
                user.RefreshToken = null;
                user.RefreshTokenExpiryTime = null;

                await _userRepository.Update(user);
                await _userRepository.SaveChange(cancellationToken);

                return new ResponseHttp
                {
                    Status = StatusCodes.Status200OK,
                    Resultat = "Password changed successfully"
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