// Application/Features/AuthFeature/Queries/GetCurrentUserQuery.cs
using Application.Features.AuthFeature.Dtos;
using Application.Interfaces;
using Application.Setting;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Features.AuthFeature.Queries
{
    public record GetCurrentUserQuery(Guid UserId) : IRequest<ResponseHttp>;

    public class GetCurrentUserQueryHandler : IRequestHandler<GetCurrentUserQuery, ResponseHttp>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public GetCurrentUserQueryHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<ResponseHttp> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
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

                var userDto = new UserDTO
                {
                    Id = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    PhoneNumber = user.PhoneNumber,
                    Role = user.Role.ToString(),
                    IsActive = user.IsActive,
                    LastLoginAt = user.LastLoginAt
                };

                return new ResponseHttp
                {
                    Resultat = userDto,
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