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
    public record GetContractDetailsQuery(Guid Id) : IRequest<ResponseHttp>;

    public class GetContractDetailsQueryHandler : IRequestHandler<GetContractDetailsQuery, ResponseHttp>
    {
        private readonly IContractRepository _repository;
        private readonly IMapper _mapper;

        public GetContractDetailsQueryHandler(IContractRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<ResponseHttp> Handle(GetContractDetailsQuery request, CancellationToken ct)
        {
            var contract = await _repository.GetByIdWithDetailsAsync(request.Id, ct);

            if (contract == null)
                return new ResponseHttp { Status = 404, Fail_Messages = "Contrat non trouvé" };

            var dto = _mapper.Map<ContractDTO>(contract); 
            return new ResponseHttp { Status = 200, Resultat = dto };
        }
    }
}
