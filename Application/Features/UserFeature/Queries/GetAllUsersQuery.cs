// Application/Features/UserFeature/Queries/GetAllUsersQuery.cs
using Application.Features.UserFeature.Dtos;
using Application.Interfaces;
using Application.Setting;
using AutoMapper;
using Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Features.UserFeature.Queries
{
    public record GetAllUsersQuery(int? PageNumber, int? PageSize, string? SortBy, bool? SortDescending, string? SearchTerm)
        : IRequest<ResponseHttp>;

    public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, ResponseHttp>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public GetAllUsersQueryHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<ResponseHttp> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var users = await _userRepository.GetAllWithFiltersAsync(
                    request.PageNumber,
                    request.PageSize,
                    request.SortBy,
                    request.SortDescending ?? false,
                    request.SearchTerm,
                    cancellationToken);

                // Vérifier si la liste est vide
                if (users == null || !users.Items.Any())
                {
                    return new ResponseHttp
                    {
                        Fail_Messages = "No users found",
                        Status = StatusCodes.Status404NotFound
                    };
                }

                // Mapper les items individuellement
                var userDtos = _mapper.Map<IEnumerable<UserDTO>>(users.Items);

                // Créer un nouveau PagedList avec les DTOs
                var usersToReturn = new PagedList<UserDTO>(
                    userDtos,
                    users.TotalCount,
                    users.CurrentPage,
                    users.PageSize
                );

                // Format de réponse attendu par le frontend (comme dans l'image)
                var response = new
                {
                    items = usersToReturn.Items,
                    totalCount = usersToReturn.TotalCount,
                    pageNumber = usersToReturn.CurrentPage,
                    pageSize = usersToReturn.PageSize
                };

                return new ResponseHttp
                {
                    Status = StatusCodes.Status200OK,
                    Resultat = response
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