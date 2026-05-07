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

            // 🔥 CORRECTION: Accepter les valeurs françaises
            RuleFor(x => x.TransportMode)
                .NotEmpty()
                .WithMessage("TransportMode est requis.")
                .Must(mode => IsValidTransportMode(mode))
                .WithMessage("TransportMode invalide. Valeurs acceptées: Maritime, Aerien, Routier, Ferroviaire, Fluvial");

            // Au moins un critère de tarification
            RuleFor(x => x)
                .Must(x =>
                    (x.WeightKg.HasValue && x.WeightKg > 0) ||
                    (x.VolumeM3.HasValue && x.VolumeM3 > 0) ||
                    (x.ContainerCount.HasValue && x.ContainerCount > 0))
                .WithMessage("Au moins un critère requis: WeightKg, VolumeM3 ou ContainerCount.");
        }

        // 🔥 CORRECTION: Accepter les valeurs françaises
        private bool IsValidTransportMode(string? mode)
        {
            if (string.IsNullOrEmpty(mode)) return false;

            var validModes = new[]
            {
                "maritime", "aerien", "routier", "ferroviaire", "fluvial",
                // Garder aussi les anglais pour compatibilité si nécessaire
                "road", "air", "rail", "sea"
            };

            return validModes.Contains(mode.ToLower());
        }
    }
}