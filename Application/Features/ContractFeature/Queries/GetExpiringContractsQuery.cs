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
    public record GetExpiringContractsQuery(int Days = 30) : IRequest<ResponseHttp>;

    public class GetExpiringContractsQueryHandler : IRequestHandler<GetExpiringContractsQuery, ResponseHttp>
    {
        private readonly IContractRepository _repository;
        private readonly IMapper _mapper;

        public GetExpiringContractsQueryHandler(IContractRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<ResponseHttp> Handle(GetExpiringContractsQuery request, CancellationToken ct)
        {
            var contracts = await _repository.GetExpiringAsync(request.Days, ct);

            var dtos = _mapper.Map<List<ContractDTO>>(contracts);

            return new ResponseHttp { Status = 200, Resultat = dtos };
        }
    }
}
