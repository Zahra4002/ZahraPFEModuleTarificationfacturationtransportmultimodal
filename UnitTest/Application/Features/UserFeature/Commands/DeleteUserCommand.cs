// Application/Features/UserFeature/Commands/DeleteUserCommand.cs
using Application.Interfaces;
using Application.Setting;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Features.UserFeature.Commands
{
    public record DeleteUserCommand(Guid UserId) : IRequest<ResponseHttp>;

    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, ResponseHttp>
    {
        private readonly IUserRepository _userRepository;

        public DeleteUserCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<ResponseHttp> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
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

                // Prevent deleting the last administrator
                if (user.Role == Domain.Enums.UserRole.Administrateur)
                {
                    var adminCount = await _userRepository.GetAdministratorCountAsync();
                    if (adminCount <= 1)
                    {
                        return new ResponseHttp
                        {
                            Fail_Messages = "Cannot delete the last administrator",
                            Status = StatusCodes.Status400BadRequest
                        };
                    }
                }

                await _userRepository.SoftDelete(request.UserId);
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