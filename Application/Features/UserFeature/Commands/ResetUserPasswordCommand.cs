using Application.Interfaces;
using Application.Setting;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Features.UserFeature.Commands
{
    public record ResetUserPasswordCommand(Guid UserId, string NewPassword) : IRequest<ResponseHttp>;

    public class ResetUserPasswordCommandHandler : IRequestHandler<ResetUserPasswordCommand, ResponseHttp>
    {
        private readonly IUserRepository _userRepository;

        public ResetUserPasswordCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<ResponseHttp> Handle(ResetUserPasswordCommand request, CancellationToken cancellationToken)
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

                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
                user.UpdatedAt = DateTime.UtcNow;

                await _userRepository.Update(user);
                await _userRepository.SaveChange(cancellationToken);

                return new ResponseHttp
                {
                    Status = StatusCodes.Status200OK,
                    Resultat = true
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