using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Features.ContractFeature.Dtos;
using Application.Interfaces;
using Application.Setting;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Features.ClientFeature.Queries
{
    public record GetContractsByClientIdQuery(Guid ClientId) : IRequest<ResponseHttp>
    {
        public class Handler : IRequestHandler<GetContractsByClientIdQuery, ResponseHttp>
        {
            private readonly IContractRepository _contractRepository;
            private readonly IMapper _mapper;

            public Handler(IContractRepository contractRepository, IMapper mapper)
            {
                _contractRepository = contractRepository;
                _mapper = mapper;
            }

            public async Task<ResponseHttp> Handle(GetContractsByClientIdQuery request, CancellationToken ct)
            {
                try
                {
                    var contracts = await _contractRepository
                        .GetContractsByClientIdAsync(request.ClientId, ct);

                    if (!contracts.Any())
                    {
                        return new ResponseHttp
                        {
                            Status = 404,
                            Fail_Messages = $"Aucun contrat trouvé pour le client ID {request.ClientId}"
                        };
                    }

                    var dtos = _mapper.Map<List<ContractDTO>>(contracts);

                    return new ResponseHttp
                    {
                        Status = 200,
                        Resultat = dtos
                    };
                }
                catch (Exception ex)
                {
                    return new ResponseHttp
                    {
                        Status = StatusCodes.Status500InternalServerError,
                        Fail_Messages = "Erreur lors de la récupération des contrats du client."
                    };
                }
            }
        }
    }
}
