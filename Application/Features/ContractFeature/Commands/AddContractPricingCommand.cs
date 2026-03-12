using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Features.ContractFeature.Dtos;
using Application.Interfaces;
using Application.Setting;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Features.ContractFeature.Commands
{
    public record AddContractPricingCommand(
          Guid ContractId,
          Guid? ZoneFromId,
          Guid? ZoneToId,
          TransportMode? TransportMode,
          bool UseFixedPrice,
          decimal? FixedPrice,
          decimal DiscountPercent,
          decimal? VolumeThreshold,
          decimal? VolumeDiscountPercent,
          string CurrencyCode = "EUR",
          bool IsActive = true,
          string? Notes = null
      ) : IRequest<ResponseHttp>;

    public class AddContractPricingCommandHandler : IRequestHandler<AddContractPricingCommand, ResponseHttp>
    {
        private readonly IContractRepository _contractRepository;
        private readonly IMapper _mapper;

        public AddContractPricingCommandHandler(IContractRepository contractRepository, IMapper mapper)
        {
            _contractRepository = contractRepository;
            _mapper = mapper;
        }

        public async Task<ResponseHttp> Handle(AddContractPricingCommand request, CancellationToken ct)
        {
            try
            {
                // 1. Vérifier que le contrat existe (sans le tracker)
                var contractExists = await _contractRepository.ExistsAsync(request.ContractId, ct);

                if (!contractExists)
                {
                    return new ResponseHttp
                    {
                        Status = StatusCodes.Status404NotFound,
                        Fail_Messages = "Contrat non trouvé"
                    };
                }

                // 2. Créer et ajouter directement le pricing
                var pricing = new ContractPricing
                {
                    Id = Guid.NewGuid(),
                    ContractId = request.ContractId,
                    ZoneFromId = request.ZoneFromId,
                    ZoneToId = request.ZoneToId,
                    TransportMode = request.TransportMode,
                    UseFixedPrice = request.UseFixedPrice,
                    FixedPrice = request.FixedPrice,
                    DiscountPercent = request.DiscountPercent,
                    VolumeThreshold = request.VolumeThreshold,
                    VolumeDiscountPercent = request.VolumeDiscountPercent,
                    CurrencyCode = request.CurrencyCode ?? "EUR",
                    IsActive = request.IsActive,
                    CreatedDate = DateTime.UtcNow
                };

                // 3. Ajouter directement au contexte
                await _contractRepository.AddContractPricingAsync(pricing, ct);

                // 4. Récupérer le contrat mis à jour
                var contract = await _contractRepository.GetByIdWithDetailsAsync(request.ContractId, ct);
                var resultDto = _mapper.Map<ContractDTO>(contract);

                return new ResponseHttp
                {
                    Resultat = resultDto,
                    Status = StatusCodes.Status201Created
                    
                };
            }
            catch (Exception ex)
            {
                return new ResponseHttp
                {
                    Fail_Messages = ex.Message,
                    Status = StatusCodes.Status400BadRequest
                };
            }
        }
    }
}
