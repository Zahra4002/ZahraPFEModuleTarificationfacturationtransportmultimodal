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
    public record UpdateContractCommandNew(
        Guid ContractId,
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
        int RenewalNoticeDays

        ) : IRequest<ResponseHttp>
    {
        public class UpdateContractCommandNewHandler : IRequestHandler<UpdateContractCommandNew, ResponseHttp>
        {
            private readonly IContractRepository contractRepository;
            private readonly IMapper _mapper;

            public UpdateContractCommandNewHandler(IContractRepository contractRepository, IMapper mapper)
            {
                this.contractRepository = contractRepository;
                _mapper = mapper;
            }

            public async Task<ResponseHttp> Handle(UpdateContractCommandNew request, CancellationToken cancellationToken)
            {
                Contract? contract = await contractRepository.GetById(request.ContractId);

                if (contract == null)
                {
                    return new ResponseHttp
                    {
                        Resultat = null,
                        Fail_Messages = "Contract with this Id not found.",
                        Status = StatusCodes.Status404NotFound,
                    };
                }

                try
                {
                    // Utiliser AutoMapper pour mapper les propriétés de la commande vers l'entité existante
                    _mapper.Map(request, contract);

                    await contractRepository.Update(contract);
                    await contractRepository.SaveChange(cancellationToken);

                    var contractToReturn = _mapper.Map<ContractDTO>(contract);
                    return new ResponseHttp
                    {
                        Resultat = contractToReturn,
                        Status = StatusCodes.Status200OK,
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
