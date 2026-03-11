using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces;
using Application.Setting;
using AutoMapper;
using Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Features.ContractFeature.Commands
{
    public record UpdateContractPricingCommand(
         Guid ContractId,
         Guid PricingId,
         Guid? ZoneFromId,
         Guid? ZoneToId,
         TransportMode? TransportMode,
         bool UseFixedPrice,
         decimal? FixedPrice,
         decimal DiscountPercent,
         decimal? VolumeThreshold,
         decimal? VolumeDiscountPercent,
         string CurrencyCode = "EUR",
         bool IsActive = true
     ) : IRequest<ResponseHttp>;

    public class UpdateContractPricingCommandHandler : IRequestHandler<UpdateContractPricingCommand, ResponseHttp>
    {
        private readonly IContractRepository _repository;
        private readonly IMapper _mapper;

        public UpdateContractPricingCommandHandler(IContractRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<ResponseHttp> Handle(UpdateContractPricingCommand request, CancellationToken ct)
        {
            try
            {
                // Vérifier que le contrat existe
                var contract = await _repository.GetByIdAsync(request.ContractId, ct);
                if (contract == null)
                {
                    return new ResponseHttp
                    {
                        Status = StatusCodes.Status404NotFound,
                        Fail_Messages = "Contrat non trouvé"
                    };
                }

                // Récupérer la tarification à mettre à jour
                var pricing = await _repository.GetContractPricingByIdAsync(request.ContractId, request.PricingId, ct);
                if (pricing == null)
                {
                    return new ResponseHttp
                    {
                        Status = StatusCodes.Status404NotFound,
                        Fail_Messages = "Tarification non trouvée pour ce contrat"
                    };
                }

                // Mapper les nouvelles valeurs vers l'entité existante
                _mapper.Map(request, pricing);

                _repository.Update(contract);

                // Mettre à jour la tarification
                _repository.SaveChange(ct);

                return new ResponseHttp
                {
                    Status = StatusCodes.Status200OK,
                    Fail_Messages = null
                };
            }
            catch (Exception ex)
            {
                return new ResponseHttp
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Fail_Messages = $"Erreur lors de la mise à jour de la tarification : {ex.Message}"
                };
            }
        }
    }
}
