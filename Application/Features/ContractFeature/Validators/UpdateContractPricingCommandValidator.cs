using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Features.ContractFeature.Commands;
using FluentValidation;

namespace Application.Features.ContractFeature.Validators
{
    public class UpdateContractPricingCommandValidator : AbstractValidator<UpdateContractPricingCommand>
    {
        public UpdateContractPricingCommandValidator()
        {
            // IDs obligatoires
            RuleFor(x => x.ContractId)
                .NotEmpty().WithMessage("L'ID du contrat est obligatoire");

            RuleFor(x => x.PricingId)
                .NotEmpty().WithMessage("L'ID de la tarification est obligatoire");

            // ZoneFromId et ZoneToId : au moins une zone doit rester
            RuleFor(x => x)
                .Must(x => x.ZoneFromId.HasValue || x.ZoneToId.HasValue)
                .When(x => x.ZoneFromId.HasValue || x.ZoneToId.HasValue) // seulement si modifié
                .WithMessage("Au moins une zone (départ ou arrivée) doit être spécifiée");

            // TransportMode
            RuleFor(x => x.TransportMode)
                .IsInEnum()
                .When(x => x.TransportMode.HasValue)
                .WithMessage("Mode de transport invalide");

            // UseFixedPrice et FixedPrice : cohérence
            RuleFor(x => x.FixedPrice)
                .GreaterThan(0)
                .When(x => x.UseFixedPrice)
                .WithMessage("Le prix fixe doit être positif quand 'UseFixedPrice' est activé");

            // DiscountPercent
            RuleFor(x => x.DiscountPercent)
                .InclusiveBetween(0, 100)
                .WithMessage("Le pourcentage de remise doit être entre 0 et 100%");

            // VolumeThreshold et VolumeDiscountPercent
            RuleFor(x => x.VolumeThreshold)
                .GreaterThan(0)
                .When(x => x.VolumeDiscountPercent.HasValue)
                .WithMessage("Le seuil de volume doit être positif quand une remise de volume est appliquée");

            RuleFor(x => x.VolumeDiscountPercent)
                .InclusiveBetween(0, 100)
                .When(x => x.VolumeDiscountPercent.HasValue)
                .WithMessage("La remise sur volume doit être entre 0 et 100%");

            // CurrencyCode
            RuleFor(x => x.CurrencyCode)
                .Length(3)
                .When(x => !string.IsNullOrWhiteSpace(x.CurrencyCode))
                .WithMessage("Le code devise doit être une valeur de 3 lettres (ex: EUR, USD)");
        }
    }
}
