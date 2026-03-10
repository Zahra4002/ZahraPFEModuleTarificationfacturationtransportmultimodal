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
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Features.ContractFeature.Queries
{
    public record GetContByIdNewQuery(
        Guid ContractId
        ) : IRequest<ResponseHttp>
    {
        public class GetContByIdNewQueryHandler : IRequestHandler<GetContByIdNewQuery, ResponseHttp>
        {
            private readonly IContractRepository contractRepository;
            private readonly IMapper _mapper;

            public GetContByIdNewQueryHandler(IContractRepository contractRepository, IMapper mapper)
            {
                this.contractRepository = contractRepository;
                _mapper = mapper;
            }

            public async Task<ResponseHttp> Handle(GetContByIdNewQuery request, CancellationToken cancellationToken)
            {
                try
                {
                    var contract = await contractRepository.GetByIdAsync(request.ContractId, cancellationToken);

                    if (contract == null)
                        return new ResponseHttp()
                        {
                            Status = 404,
                            Fail_Messages = "Contract not found !"
                        };

                    return new ResponseHttp()
                    {

                        Resultat = _mapper.Map<ContractDTO>(contract),
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
