using Application.Features.AuthFeature.Dtos;
using Application.Interfaces;
using Application.Setting;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Features.AuthFeature.Commands
{
    public record ForgotPasswordCommand(ForgotPasswordRequestDTO Request) : IRequest<ResponseHttp>;

    public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, ResponseHttp>
    {
        private readonly IUserRepository _userRepository;
        private readonly IEmailSender _emailSender;

        public ForgotPasswordCommandHandler(IUserRepository userRepository, IEmailSender emailSender)
        {
            _userRepository = userRepository;
            _emailSender = emailSender;
        }

        public async Task<ResponseHttp> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Request.Email))
                {
                    return new ResponseHttp
                    {
                        Resultat = "If an account with that email exists, a password reset code has been sent.",
                        Status = StatusCodes.Status200OK
                    };
                }

                var user = await _userRepository.GetByEmailAsync(request.Request.Email);

                if (user != null)
                {
                    var resetCode = GenerateResetCode();
                    var hashedCode = BCrypt.Net.BCrypt.HashPassword(resetCode);

                    user.PasswordResetCodeHash = hashedCode;
                    user.PasswordResetCodeExpiryUtc = DateTime.UtcNow.AddMinutes(10);
                    user.UpdatedAt = DateTime.UtcNow;

                    await _userRepository.Update(user);
                    await _userRepository.SaveChange(cancellationToken);

                    var emailBody = $@"
                        <h2>Demande de réinitialisation de mot de passe</h2>
                        <p>Bonjour {user.FirstName},</p>
                        <p>Nous avons reçu une demande de réinitialisation de votre mot de passe. Utilisez le code ci-dessous pour le réinitialiser :</p>
                        <h3 style='color: #007bff; font-family: monospace; letter-spacing: 3px;'>{resetCode}</h3>
                        <p>Ce code expire dans 10 minutes.</p>
                        <p>Si vous n'avez pas demandé cette réinitialisation, veuillez ignorer cet email.</p>";

                    await _emailSender.SendAsync(user.Email, "Code de réinitialisation de mot de passe", emailBody, cancellationToken);
                }

                return new ResponseHttp
                {
                    Resultat = "If an account with that email exists, a password reset code has been sent.",
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

        private string GenerateResetCode()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString();
        }
    }
}   