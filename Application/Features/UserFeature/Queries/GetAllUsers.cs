using Application.Features.UserFeature.Dtos;
using Domain.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.UserFeature.Queries
{
    public record GetAllUsersQuery(int? PageNumber, int? PageSize) : IRequest<PagedList<UserDTO>>;
    
    public class GetAllUsersHandler : IRequestHandler<GetAllUsersQuery, PagedList<UserDTO>>
    {
        public async Task<PagedList<UserDTO>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
