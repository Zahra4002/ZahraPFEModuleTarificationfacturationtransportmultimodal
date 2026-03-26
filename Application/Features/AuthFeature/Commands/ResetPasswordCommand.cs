using Application.Features.AuthFeature.Dtos;
using Application.Interfaces;
using Application.Setting;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Features.AuthFeature.Commands
{
    public record ResetPasswordCommand(ResetPasswordRequestDTO Request) : IRequest<ResponseHttp>;

    public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, ResponseHttp>
    {
        private readonly IUserRepository _userRepository;

        public ResetPasswordCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<ResponseHttp> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (request.Request.NewPassword != request.Request.ConfirmNewPassword)
                {
                    return new ResponseHttp
                    {
                        Fail_Messages = "Passwords do not match",
                        Status = StatusCodes.Status400BadRequest
                    };
                }

                if (string.IsNullOrWhiteSpace(request.Request.Email) || string.IsNullOrWhiteSpace(request.Request.Code))
                {
                    return new ResponseHttp
                    {
                        Fail_Messages = "Email and code are required",
                        Status = StatusCodes.Status400BadRequest
                    };
                }

                var user = await _userRepository.GetByEmailAsync(request.Request.Email);

                if (user == null || string.IsNullOrEmpty(user.PasswordResetCodeHash))
                {
                    return new ResponseHttp
                    {
                        Fail_Messages = "Invalid email or reset code",
                        Status = StatusCodes.Status400BadRequest
                    };
                }

                if (user.PasswordResetCodeExpiryUtc <= DateTime.UtcNow)
                {
                    return new ResponseHttp
                    {
                        Fail_Messages = "Reset code has expired",
                        Status = StatusCodes.Status400BadRequest
                    };
                }

                if (!BCrypt.Net.BCrypt.Verify(request.Request.Code, user.PasswordResetCodeHash))
                {
                    return new ResponseHttp
                    {
                        Fail_Messages = "Invalid reset code",
                        Status = StatusCodes.Status400BadRequest
                    };
                }

                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Request.NewPassword);
                user.PasswordResetCodeHash = null;
                user.PasswordResetCodeExpiryUtc = null;
                user.RefreshToken = null;
                user.RefreshTokenExpiryTime = null;
                user.UpdatedAt = DateTime.UtcNow;

                await _userRepository.Update(user);
                await _userRepository.SaveChange(cancellationToken);

                return new ResponseHttp
                {
                    Resultat = "Password reset successfully",
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