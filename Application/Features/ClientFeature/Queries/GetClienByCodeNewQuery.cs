using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Features.TestFeature.Dtos;
using Application.Interfaces;
using Application.Setting;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Features.ClientFeature.Queries
{
    public record GetClienByCodeNewQuery(
        string Code
        ) : IRequest<ResponseHttp>
    {
        public class GetClienByCodeNewQueryHandler : IRequestHandler<GetClienByCodeNewQuery, ResponseHttp>
        {
            private readonly IClientRepository clientRepository;
            private readonly IMapper _mapper;

            public GetClienByCodeNewQueryHandler(IClientRepository clientRepository, IMapper mapper)
            {
                this.clientRepository = clientRepository;
                _mapper = mapper;
            }

            public async Task<ResponseHttp> Handle(GetClienByCodeNewQuery request, CancellationToken cancellationToken)
            {
                try
                {
                    var client = await clientRepository.GetByCodeAsync(request.Code, cancellationToken);

                    if (client == null)
                        return new ResponseHttp()
                        {
                            Status = 404,
                            Fail_Messages = "Client not found !"
                        };

                    return new ResponseHttp()
                    {

                        Resultat = _mapper.Map<ClientDTO>(client),
                        Status = 200
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
        }
    }
}
