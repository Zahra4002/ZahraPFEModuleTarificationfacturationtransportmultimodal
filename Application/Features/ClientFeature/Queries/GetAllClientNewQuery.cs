using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Features.TestFeature.Dtos;
using Application.Features.TestFeature.Queries;
using Application.Interfaces;
using Application.Setting;
using AutoMapper;
using Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Features.ClientFeature.Queries
{
    public record GetAllClientNewQuery(int? pageNumber, int? pageSize) : IRequest<ResponseHttp>
    {
        public class GetAllClientNewQueryHandler : IRequestHandler<GetAllClientNewQuery, ResponseHttp>
        {
            private readonly IClientRepository clientRepository;
            private readonly IMapper _mapper;

            public GetAllClientNewQueryHandler(IClientRepository clientRepository, IMapper mapper)
            {
                this.clientRepository = clientRepository;
                _mapper = mapper;
            }

            public async Task<ResponseHttp> Handle(GetAllClientNewQuery request, CancellationToken cancellationToken)
            {
                var client = await clientRepository.GetAllWithTypesAsync(request.pageNumber, request.pageSize, cancellationToken);

                if (client == null)
                    return new ResponseHttp
                    {
                        Fail_Messages = "No client found !",
                        Status = StatusCodes.Status400BadRequest,
                    };

                var customersToReturn = _mapper.Map<PagedList<ClientDTO>>(client);
                return new ResponseHttp
                {
                    Status = 200,
                    Resultat = customersToReturn
                };
            }
        }
    }
}
