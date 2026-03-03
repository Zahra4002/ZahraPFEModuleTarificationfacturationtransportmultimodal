// Application/Features/TariffGridFeature/Validators/CreateTariffGridCommandValidator.cs
using Application.Common.Validator;
using Application.Features.TariffGridFeature.Commands;
using FluentValidation;

namespace Application.Features.TariffGridFeature.Validators
{
    public class CreateTariffGridCommandValidator : ValidatorBase<CreateTariffGridCommand>
    {
        public CreateTariffGridCommandValidator()
        {
            RuleFor(v => v.TariffGridDto.Code)
                .NotEmpty().WithMessage("Code is required")
                .MaximumLength(50).WithMessage("Code must not exceed 50 characters");

            RuleFor(v => v.TariffGridDto.Name)
                .NotEmpty().WithMessage("Name is required")
                .MaximumLength(200).WithMessage("Name must not exceed 200 characters");

            RuleFor(v => v.TariffGridDto.TransportMode)
                .NotEmpty().WithMessage("Transport mode is required")
                .Must(mode => new[] { "Maritime", "Aerien", "Routier", "Ferroviaire", "Fluvial", "Terrestre", "RoRo", "Conteneurise" }
                    .Contains(mode, StringComparer.OrdinalIgnoreCase))
                .WithMessage("Invalid transport mode");

            RuleFor(v => v.TariffGridDto.ValidFrom)
                .NotEmpty().WithMessage("Valid from date is required");

            RuleFor(v => v.TariffGridDto.CurrencyCode)
                .NotEmpty().WithMessage("Currency code is required")
                .Length(3).WithMessage("Currency code must be 3 characters");

            // Correction ici : utiliser TariffGridDto.ValidFrom au lieu de dto.ValidFrom
            RuleFor(v => v.TariffGridDto.ValidTo)
                .Must((cmd, validTo) => !validTo.HasValue || validTo.Value > cmd.TariffGridDto.ValidFrom)
                .WithMessage("Valid to date must be after valid from date");
        }
    }
}