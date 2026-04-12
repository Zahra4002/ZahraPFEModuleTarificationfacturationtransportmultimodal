using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Features.TestFeature.Dtos;
using Application.Interfaces;
using Application.Setting;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Http;
using BCrypt.Net;

namespace Application.Features.ClientFeature.Commands
{
    public record AddClientCommandNew(
        string code,
        string name,
        string taxId,
        string email,
        string phoneNumber,
        Address bullingAddress,
        Address shippingAddress,
        string defaultCurrencyCode,
        decimal creditLimit,
        int paymenttermDays,
        bool isActive
        ) : IRequest<ResponseHttp>
    {
        public class AddClientCommandNewHandler : IRequestHandler<AddClientCommandNew, ResponseHttp>
        {
            private readonly IClientRepository clientRepository;
            private readonly IUserRepository _userRepository;
            private readonly IEmailSender _emailSender;
            private readonly IMapper _mapper;

            public AddClientCommandNewHandler(
                IClientRepository clientRepository,
                IUserRepository userRepository,
                IEmailSender emailSender,
                IMapper mapper)
            {
                this.clientRepository = clientRepository;
                _userRepository = userRepository;
                _emailSender = emailSender;
                _mapper = mapper;
            }

            public async Task<ResponseHttp> Handle(AddClientCommandNew request, CancellationToken cancellationToken)
            {
                try
                {
                    // 1️⃣ Vérifier si l'email existe déjà
                    var existingUser = await _userRepository.GetByEmailAsync(request.email);
                    if (existingUser != null)
                    {
                        return new ResponseHttp
                        {
                            Fail_Messages = "Un utilisateur avec cet email existe déjà.",
                            Status = StatusCodes.Status400BadRequest,
                        };
                    }

                    // 2️⃣ Créer le client
                    var client = _mapper.Map<Client>(request);
                    client = await clientRepository.Post(client);

                    // 3️⃣ Générer un mot de passe aléatoire
                    var plainPassword = GenerateRandomPassword();
                    var passwordHash = BCrypt.Net.BCrypt.HashPassword(plainPassword);

                    // 4️⃣ Créer l'utilisateur associé
                    var user = new User
                    {
                        Id = Guid.NewGuid(),
                        Email = request.email,
                        PasswordHash = passwordHash,
                        FirstName = request.name,
                        LastName = request.name,
                        PhoneNumber = request.phoneNumber,
                        Role = UserRole.Lecture,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    };

                    await _userRepository.AddAsync(user);
                    await _userRepository.SaveChange(cancellationToken);

                    // 5️⃣ Envoyer l'email avec le mot de passe
                    await SendWelcomeEmail(request.email, request.name, plainPassword, cancellationToken);

                    await clientRepository.SaveChange(cancellationToken);

                    return new ResponseHttp()
                    {
                        Resultat = _mapper.Map<ClientDTO>(client),
                        Status = 200,
                        Fail_Messages = $"Client créé avec succès. Un email a été envoyé à {request.email} avec ses identifiants."
                    };
                }
                catch (Exception ex)
                {
                    return new ResponseHttp
                    {
                        Fail_Messages = ex.Message,
                        Status = StatusCodes.Status400BadRequest,
                    };
                }
            }

            private string GenerateRandomPassword(int length = 10)
            {
                const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz0123456789!@#$%";
                var random = new Random();
                return new string(Enumerable.Repeat(chars, length)
                    .Select(s => s[random.Next(s.Length)]).ToArray());
            }

            private async Task SendWelcomeEmail(string email, string clientName, string password, CancellationToken cancellationToken)
            {
                var subject = "🚚 TransForward - Vos identifiants de connexion";

                var body = $@"
                    <!DOCTYPE html>
                    <html>
                    <head>
                        <style>
                            body {{ font-family: Arial, sans-serif; }}
                            .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                            .header {{ background: #4CAF50; color: white; padding: 20px; text-align: center; border-radius: 8px 8px 0 0; }}
                            .content {{ padding: 20px; background: #f9f9f9; }}
                            .credentials {{ background: white; padding: 15px; border-radius: 8px; margin: 20px 0; border-left: 4px solid #4CAF50; }}
                            .password {{ font-size: 20px; font-weight: bold; color: #4CAF50; font-family: monospace; }}
                            .footer {{ text-align: center; padding: 20px; font-size: 12px; color: #666; }}
                            .btn {{ background: #4CAF50; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px; }}
                        </style>
                    </head>
                    <body>
                        <div class='container'>
                            <div class='header'>
                                <h1>🚚 TransForward</h1>
                                <p>Votre plateforme de transport et logistique</p>
                            </div>
                            <div class='content'>
                                <h2>Bonjour {clientName},</h2>
                                <p>Nous avons le plaisir de vous accueillir sur <strong>TransForward</strong> !</p>
                                <p>Votre compte client a été créé avec succès. Voici vos identifiants de connexion :</p>
                                
                                <div class='credentials'>
                                    <p><strong>📧 Email :</strong> {email}</p>
                                    <p><strong>🔑 Mot de passe :</strong> <span class='password'>{password}</span></p>
                                </div>
                                
                                <p><strong>⚠️ Important :</strong> Nous vous recommandons de changer votre mot de passe lors de votre première connexion.</p>
                                
                                <p style='text-align: center; margin: 30px 0;'>
                                    <a href='https://votre-app.com/connexion' class='btn'>Se connecter</a>
                                </p>
                                
                                <p>📱 <strong>Application mobile :</strong> Téléchargez l'application TransForward sur l'App Store ou Google Play.</p>
                                <p>💻 <strong>Espace web :</strong> Accédez à votre espace client sur <a href='https://votre-app.com'>www.transforward.com</a></p>
                            </div>
                            <div class='footer'>
                                <p>© 2024 TransForward - Tous droits réservés</p>
                                <p>Cet email a été généré automatiquement, merci de ne pas y répondre.</p>
                            </div>
                        </div>
                    </body>
                    </html>";

                await _emailSender.SendAsync(email, subject, body, cancellationToken);
            }
        }
    }
}