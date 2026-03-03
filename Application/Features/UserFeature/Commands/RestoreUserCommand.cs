using Application.Interfaces;
using Application.Setting;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Features.UserFeature.Commands
{
    public record RestoreUserCommand(Guid UserId) : IRequest<ResponseHttp>;

    public class RestoreUserCommandHandler : IRequestHandler<RestoreUserCommand, ResponseHttp>
    {
        private readonly IUserRepository _userRepository;

        public RestoreUserCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<ResponseHttp> Handle(RestoreUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // ⚠️ PROBLEME: GetByIdAsync filtre les utilisateurs supprimés !
                // var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

                // ✅ SOLUTION: Utiliser GetByIdIncludingDeleted (à créer)
                var user = await _userRepository.GetByIdIncludingDeletedAsync(request.UserId, cancellationToken);

                if (user == null)
                {
                    return new ResponseHttp
                    {
                        Fail_Messages = "User not found",
                        Status = StatusCodes.Status404NotFound
                    };
                }

                // Vérifier que l'utilisateur est bien supprimé
                if (!user.IsDeleted)
                {
                    return new ResponseHttp
                    {
                        Fail_Messages = "User is not deleted",
                        Status = StatusCodes.Status400BadRequest
                    };
                }

                var result = await _userRepository.RestoreUser(request.UserId);
                await _userRepository.SaveChange(cancellationToken);

                if (result)
                {
                    return new ResponseHttp
                    {
                        Status = StatusCodes.Status200OK,
                        Resultat = "User restored successfully"
                    };
                }
                else
                {
                    return new ResponseHttp
                    {
                        Fail_Messages = "Failed to restore user",
                        Status = StatusCodes.Status400BadRequest
                    };
                }
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