using Application.Features.UserFeature.Dtos;
using Application.Interfaces;
using Application.Setting;
using AutoMapper;
using Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Features.UserFeature.Commands
{
    public record UpdateUserCommand(UpdateUserDTO UserDto) : IRequest<ResponseHttp>;

    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, ResponseHttp>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UpdateUserCommandHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<ResponseHttp> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(request.UserDto.Id, cancellationToken);
                if (user == null)
                {
                    return new ResponseHttp
                    {
                        Fail_Messages = "User not found",
                        Status = StatusCodes.Status404NotFound
                    };
                }

                // Check if email is being changed and if it's already taken
                if (user.Email != request.UserDto.Email)
                {
                    var existingUser = await _userRepository.GetByEmailAsync(request.UserDto.Email);
                    if (existingUser != null)
                    {
                        return new ResponseHttp
                        {
                            Fail_Messages = "Email already in use by another user",
                            Status = StatusCodes.Status400BadRequest
                        };
                    }
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

                user.Email = request.UserDto.Email;
                user.FirstName = request.UserDto.FirstName;
                user.LastName = request.UserDto.LastName;
                user.PhoneNumber = request.UserDto.PhoneNumber;
                user.ProfilePicture = request.UserDto.ProfilePicture;
                user.Role = role;
                user.IsActive = request.UserDto.IsActive;
                user.UpdatedAt = DateTime.UtcNow;

                await _userRepository.Update(user);
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