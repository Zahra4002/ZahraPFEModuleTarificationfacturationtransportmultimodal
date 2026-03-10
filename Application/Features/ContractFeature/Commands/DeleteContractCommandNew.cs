using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces;
using Application.Setting;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Features.ContractFeature.Commands
{
    public record DeleteContractCommandNew(
       Guid ContractId
       )
       : IRequest<ResponseHttp>
    {

        public class DeleteContractCommandNewHandler : IRequestHandler<DeleteContractCommandNew, ResponseHttp>
        {
            private readonly IContractRepository contractRepository;
            public DeleteContractCommandNewHandler(IContractRepository contractRepository)
            {
                this.contractRepository = contractRepository;
            }

            public async Task<ResponseHttp> Handle(DeleteContractCommandNew request, CancellationToken cancellationToken)
            {
                var contract = await contractRepository.GetById(request.ContractId);

                if (contract == null)
                {
                    return new ResponseHttp
                    {
                        Fail_Messages = "No test found",
                        Status = StatusCodes.Status400BadRequest,
                    };
                }

                await contractRepository.SoftDelete(request.ContractId);
                await contractRepository.SaveChange(cancellationToken);

                return new ResponseHttp
                {
                    Status = StatusCodes.Status200OK,
                };
            }
        }
    }
    }
