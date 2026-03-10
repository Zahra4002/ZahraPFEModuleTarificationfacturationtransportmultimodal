using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces;
using Application.Setting;
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

        public UpdateContractPricingCommandHandler(IContractRepository repository)
        {
            _repository = repository;
        }

        public async Task<ResponseHttp> Handle(UpdateContractPricingCommand request, CancellationToken ct)
        {
            try
            {
                var contract = await _repository.GetByIdAsync(request.ContractId, ct);

                if (contract == null)
                {
                    return new ResponseHttp
                    {
                        Status = StatusCodes.Status404NotFound,
                        Fail_Messages = "Contrat non trouvé"
                    };
                }

                var pricing = contract.ContractPricings.FirstOrDefault(p => p.Id == request.PricingId);

                if (pricing == null)
                {
                    return new ResponseHttp
                    {
                        Status = StatusCodes.Status404NotFound,
                        Fail_Messages = "Tarification non trouvée"
                    };
                }

                // تحديث الحقول الموجودة فقط (اللي في ContractPricing)
                pricing.ZoneFromId = request.ZoneFromId;
                pricing.ZoneToId = request.ZoneToId;
                pricing.TransportMode = request.TransportMode;

                pricing.UseFixedPrice = request.UseFixedPrice;
                pricing.FixedPrice = request.FixedPrice;
                pricing.DiscountPercent = request.DiscountPercent;

                pricing.VolumeThreshold = request.VolumeThreshold;
                pricing.VolumeDiscountPercent = request.VolumeDiscountPercent;

                pricing.CurrencyCode = request.CurrencyCode ?? "EUR";
                pricing.IsActive = request.IsActive;

                await _repository.UpdateAsync(contract, ct);
                await _repository.SaveChangesAsync(ct);

                return new ResponseHttp
                {
                    Status = StatusCodes.Status200OK,
                    Resultat = "Tarification mise à jour avec succès"
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
