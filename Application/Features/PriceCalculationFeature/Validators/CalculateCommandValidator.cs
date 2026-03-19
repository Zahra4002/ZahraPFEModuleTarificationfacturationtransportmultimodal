using Application.Features.PriceCalculationFeature.Commands;
using FluentValidation;
using System;

namespace Application.Features.PriceCalculationFeature.Validators
{
    public class CalculatePriceCommandValidator : AbstractValidator<CalculatePriceCommand>
    {
        public CalculatePriceCommandValidator()
        {
            // Client
            RuleFor(x => x.ClientId)
                .NotEmpty()
                .WithMessage("ClientId est requis.");

            // Zones
            RuleFor(x => x.ZoneFromId)
                .NotEmpty()
                .WithMessage("ZoneFromId est requis.");

            RuleFor(x => x.ZoneToId)
                .NotEmpty()
                .WithMessage("ZoneToId est requis.");

            RuleFor(x => x)
                .Must(x => x.ZoneFromId != x.ZoneToId)
                .WithMessage("Les zones de départ et d'arrivée doivent être différentes.");

            // Transport Mode
            RuleFor(x => x.TransportMode)
                .NotEmpty()
                .WithMessage("TransportMode est requis.")
                .Must(mode => IsValidTransportMode(mode))
                .WithMessage("TransportMode invalide (Road, Maritime, Air, Rail).");

            // Au moins un critère de tarification
            RuleFor(x => x)
                .Must(x => 
                    (x.WeightKg.HasValue && x.WeightKg > 0) ||
                    (x.VolumeM3.HasValue && x.VolumeM3 > 0) ||
                    (x.ContainerCount.HasValue && x.ContainerCount > 0))
                .WithMessage("Au moins un critère requis: WeightKg, VolumeM3 ou ContainerCount.");
        }

        private bool IsValidTransportMode(string? mode)
        {
            if (string.IsNullOrEmpty(mode)) return false;
            return mode.ToLower() is "road" or "maritime" or "air" or "rail";
        }
    }
}
