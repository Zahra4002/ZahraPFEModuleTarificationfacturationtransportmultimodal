using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Features.ContractFeature.Dtos;
using Application.Features.TestFeature.Dtos;
using Application.Interfaces;
using Application.Setting;
using AutoMapper;
using Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Features.ContractFeature.Queries
{
    public record GetAllContractNewQuery(int? pageNumber, int? pageSize) : IRequest<ResponseHttp>
    {
        public class GetAllContractNewQueryHandler : IRequestHandler<GetAllContractNewQuery, ResponseHttp>
        {
            private readonly IContractRepository contractRepository;
            private readonly IMapper _mapper;

            public GetAllContractNewQueryHandler(IContractRepository contractRepository, IMapper mapper)
            {
                this.contractRepository = contractRepository;
                _mapper = mapper;
            }

            public async Task<ResponseHttp> Handle(GetAllContractNewQuery request, CancellationToken cancellationToken)
            {
                var contract = await contractRepository.GetAllWithTypesAsync(request.pageNumber, request.pageSize, cancellationToken);

                if (contract == null)
                    return new ResponseHttp
                    {
                        Fail_Messages = "No contract found !",
                        Status = StatusCodes.Status400BadRequest,
                    };

                var customersToReturn = _mapper.Map<PagedList<ContractDTO>>(contract);
                return new ResponseHttp
                {
                    Status = 200,
                    Resultat = customersToReturn
                };
            }
        }
    }
}
