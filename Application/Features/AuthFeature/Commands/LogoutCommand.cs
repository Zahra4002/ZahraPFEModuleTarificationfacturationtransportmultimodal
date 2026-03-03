using Application.Interfaces;
using Application.Setting;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Features.AuthFeature.Commands
{
    public record LogoutCommand(Guid UserId) : IRequest<ResponseHttp>;

    public class LogoutCommandHandler : IRequestHandler<LogoutCommand, ResponseHttp>
    {
        private readonly IUserRepository _userRepository;

        public LogoutCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<ResponseHttp> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
                if (user != null)
                {
                    // Invalidate refresh token
                    user.RefreshToken = null;
                    user.RefreshTokenExpiryTime = null;
                    await _userRepository.Update(user);
                    await _userRepository.SaveChange(cancellationToken);
                }

                return new ResponseHttp
                {
                    Status = StatusCodes.Status200OK,
                    Resultat = "Logged out successfully"
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