// Application/Features/ClientFeature/Commands/ResendClientCredentialsCommand.cs
using Application.Interfaces;
using Application.Setting;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Features.ClientFeature.Commands
{
    public record ResendClientCredentialsCommand(Guid ClientId) : IRequest<ResponseHttp>;

    public class ResendClientCredentialsCommandHandler : IRequestHandler<ResendClientCredentialsCommand, ResponseHttp>
    {
        private readonly IClientRepository _clientRepository;
        private readonly IUserRepository _userRepository;
        private readonly IEmailSender _emailSender;

        public ResendClientCredentialsCommandHandler(
            IClientRepository clientRepository,
            IUserRepository userRepository,
            IEmailSender emailSender)
        {
            _clientRepository = clientRepository;
            _userRepository = userRepository;
            _emailSender = emailSender;
        }

        public async Task<ResponseHttp> Handle(ResendClientCredentialsCommand request, CancellationToken cancellationToken)
        {
            var client = await _clientRepository.GetByIdAsync(request.ClientId, cancellationToken);
            if (client == null)
                return new ResponseHttp { Status = 404, Fail_Messages = "Client non trouvé" };

            var user = await _userRepository.GetByEmailAsync(client.Email);
            if (user == null)
                return new ResponseHttp { Status = 404, Fail_Messages = "Utilisateur non trouvé" };

            // Générer un nouveau mot de passe
            var newPassword = GenerateRandomPassword();
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            await _userRepository.Update(user);
            await _userRepository.SaveChange(cancellationToken);

            // Envoyer l'email
            await SendCredentialsEmail(client.Email, client.Name, newPassword, cancellationToken);

            return new ResponseHttp { Status = 200, Resultat = "Email envoyé avec succès" };
        }

        private string GenerateRandomPassword(int length = 10)
        {
            const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz0123456789!@#$%";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private async Task SendCredentialsEmail(string email, string clientName, string password, CancellationToken cancellationToken)
        {
            // Même template que ci-dessus
            var subject = "TransForward - Vos identifiants de connexion";
            var body = $@"..."; // Template HTML similaire
            await _emailSender.SendAsync(email, subject, body, cancellationToken);
        }
    }
}