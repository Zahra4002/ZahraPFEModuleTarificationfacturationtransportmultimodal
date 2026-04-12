// Application/Features/ClientFeature/Commands/ChangeClientPasswordCommand.cs
using Application.Interfaces;
using Application.Setting;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Features.ClientFeature.Commands
{
    public record ChangeClientPasswordCommand(
        Guid UserId,
        string CurrentPassword,
        string NewPassword
    ) : IRequest<ResponseHttp>;

    public class ChangeClientPasswordCommandHandler : IRequestHandler<ChangeClientPasswordCommand, ResponseHttp>
    {
        private readonly IUserRepository _userRepository;

        public ChangeClientPasswordCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<ResponseHttp> Handle(ChangeClientPasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
            if (user == null)
                return new ResponseHttp { Status = 404, Fail_Messages = "Utilisateur non trouvé" };

            if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.PasswordHash))
                return new ResponseHttp { Status = 400, Fail_Messages = "Mot de passe actuel incorrect" };

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            await _userRepository.Update(user);
            await _userRepository.SaveChange(cancellationToken);

            return new ResponseHttp { Status = 200, Resultat = "Mot de passe modifié avec succès" };
        }
    }
}