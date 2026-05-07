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
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Features.ContractFeature.Commands
{
    public record AddContractCommandNew(
        
        string contractNumber,
        string name,
        ContractType Type,
        DateTime validForm,
        DateTime validTo,
        Guid ClientId,  
        Guid SupplierId,
        string terms,
        bool termsAccepted,
        DateTime termsAccptedAt,
        decimal minimumVolume,
        string minimumVolumeUnit,
        bool IsActive,
        bool AutoRenew,
        int RenewalNoticeDays,
        decimal GlobalDiscountPercent
        ) : IRequest<ResponseHttp>
    {
        public class AddContractCommandNewHandler : IRequestHandler<AddContractCommandNew, ResponseHttp>
        {
            private readonly IContractRepository contractRepository;
            private readonly IMapper _mapper;

            public AddContractCommandNewHandler(IContractRepository contractRepository, IMapper mapper)
            {
                this.contractRepository = contractRepository;
                _mapper = mapper;
            }

            public async Task<ResponseHttp> Handle(AddContractCommandNew request, CancellationToken cancellationToken)
            {
                try
                {
                    var contract = _mapper.Map<Contract>(request);
                    contract.GlobalDiscountPercent = request.GlobalDiscountPercent;
                    contract = await contractRepository.Post(contract);
                    await contractRepository.SaveChange(cancellationToken);

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
