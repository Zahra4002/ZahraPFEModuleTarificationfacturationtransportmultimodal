using Application.Interfaces;
using Application.Setting;
using Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Features.UserFeature.Commands
{
    public record UpdateUserRoleCommand(Guid UserId, string Role) : IRequest<ResponseHttp>;

    public class UpdateUserRoleCommandHandler : IRequestHandler<UpdateUserRoleCommand, ResponseHttp>
    {
        private readonly IUserRepository _userRepository;

        public UpdateUserRoleCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<ResponseHttp> Handle(UpdateUserRoleCommand request, CancellationToken cancellationToken)
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

                if (!Enum.TryParse<UserRole>(request.Role, true, out var role))
                {
                    return new ResponseHttp
                    {
                        Fail_Messages = "Invalid role specified",
                        Status = StatusCodes.Status400BadRequest
                    };
                }

                // Prevent changing role of last administrator
                if (user.Role == UserRole.Administrateur && role != UserRole.Administrateur)
                {
                    var adminCount = await _userRepository.GetAdministratorCountAsync();
                    if (adminCount <= 1)
                    {
                        return new ResponseHttp
                        {
                            Fail_Messages = "Cannot change role of the last administrator",
                            Status = StatusCodes.Status400BadRequest
                        };
                    }
                }

                user.Role = role;
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