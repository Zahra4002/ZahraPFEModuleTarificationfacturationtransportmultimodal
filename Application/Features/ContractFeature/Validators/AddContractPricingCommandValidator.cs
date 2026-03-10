using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Features.ContractFeature.Commands;
using FluentValidation;

namespace Application.Features.ContractFeature.Validators
{
    public class AddContractPricingCommandValidator : AbstractValidator<AddContractPricingCommand>
    {
        public AddContractPricingCommandValidator()
        {
            // ContractId obligatoire
            RuleFor(x => x.ContractId)
                .NotEmpty().WithMessage("L'ID du contrat est obligatoire");

            // ZoneFromId et ZoneToId : au moins une zone doit être fournie
            RuleFor(x => x)
                .Must(x => x.ZoneFromId.HasValue || x.ZoneToId.HasValue)
                .WithMessage("Au moins une zone (départ ou arrivée) doit être spécifiée");

            // TransportMode : obligatoire si zones renseignées
            RuleFor(x => x.TransportMode)
                .NotNull()
                .When(x => x.ZoneFromId.HasValue || x.ZoneToId.HasValue)
                .WithMessage("Le mode de transport est obligatoire quand des zones sont définies");

            RuleFor(x => x.TransportMode)
                .IsInEnum()
                .When(x => x.TransportMode.HasValue)
                .WithMessage("Mode de transport invalide");

            // UseFixedPrice et FixedPrice : cohérence
            RuleFor(x => x.FixedPrice)
                .GreaterThan(0)
                .When(x => x.UseFixedPrice)
                .WithMessage("Le prix fixe doit être positif quand 'UseFixedPrice' est activé");

            RuleFor(x => x.FixedPrice)
                .Null()
                .When(x => !x.UseFixedPrice)
                .WithMessage("Le prix fixe doit être null quand 'UseFixedPrice' est désactivé");

            // DiscountPercent : entre 0 et 100
            RuleFor(x => x.DiscountPercent)
                .InclusiveBetween(0, 100)
                .WithMessage("Le pourcentage de remise doit être entre 0 et 100%");

            // VolumeThreshold et VolumeDiscountPercent : cohérence
            RuleFor(x => x.VolumeThreshold)
                .GreaterThan(0)
                .When(x => x.VolumeDiscountPercent.HasValue)
                .WithMessage("Le seuil de volume doit être positif quand une remise de volume est appliquée");

            RuleFor(x => x.VolumeDiscountPercent)
                .InclusiveBetween(0, 100)
                .When(x => x.VolumeDiscountPercent.HasValue)
                .WithMessage("La remise sur volume doit être entre 0 et 100%");

            // CurrencyCode : obligatoire et 3 lettres
            RuleFor(x => x.CurrencyCode)
                .NotEmpty()
                .Length(3)
                .WithMessage("Le code devise doit être une valeur de 3 lettres (ex: EUR, USD)");
        }
    }
}
