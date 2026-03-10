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

namespace Application.Features.ContractFeature.Queries
{
    public record GetContractsByClientQuery(Guid ClientId) : IRequest<ResponseHttp>;

    public class GetContractsByClientQueryHandler : IRequestHandler<GetContractsByClientQuery, ResponseHttp>
    {
        private readonly IContractRepository _repository;
        private readonly IMapper _mapper;

        public GetContractsByClientQueryHandler(IContractRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<ResponseHttp> Handle(GetContractsByClientQuery request, CancellationToken ct)
        {
            var contracts = await _repository.GetByClientIdAsync(request.ClientId, ct);

            if (!contracts.Any())
            {
                return new ResponseHttp
                {
                    Status = 404,
                    Fail_Messages = "Aucun contrat trouvé pour ce client"
                };
            }

            var dtos = _mapper.Map<List<ContractDTO>>(contracts);

            return new ResponseHttp
            {
                Status = 200,
                Resultat = dtos
            };
        }
    }
}
