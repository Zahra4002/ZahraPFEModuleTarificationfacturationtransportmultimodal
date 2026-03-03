using Application.Features.UserFeature.Dtos;
using Application.Interfaces;
using Application.Setting;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Features.UserFeature.Commands
{
    public record AddUserCommand(CreateUserDTO UserDto) : IRequest<ResponseHttp>;

    public class AddUserCommandHandler : IRequestHandler<AddUserCommand, ResponseHttp>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public AddUserCommandHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<ResponseHttp> Handle(AddUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Check if user already exists
                var existingUser = await _userRepository.GetByEmailAsync(request.UserDto.Email);
                if (existingUser != null)
                {
                    return new ResponseHttp
                    {
                        Fail_Messages = "User with this email already exists",
                        Status = StatusCodes.Status400BadRequest
                    };
                }

                // Parse role
                if (!Enum.TryParse<UserRole>(request.UserDto.Role, true, out var role))
                {
                    return new ResponseHttp
                    {
                        Fail_Messages = "Invalid role specified",
                        Status = StatusCodes.Status400BadRequest
                    };
                }

                var user = new User
                {
                    Email = request.UserDto.Email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.UserDto.Password),
                    FirstName = request.UserDto.FirstName,
                    LastName = request.UserDto.LastName,
                    PhoneNumber = request.UserDto.PhoneNumber,
                    Role = role,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                await _userRepository.AddAsync(user);
                await _userRepository.SaveChange(cancellationToken);

                return new ResponseHttp
                {
                    Resultat = _mapper.Map<UserDTO>(user),
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