// Application/Features/TariffGridFeature/Validators/UpdateTariffGridCommandValidator.cs
using Application.Common.Validator;
using Application.Features.TariffGridFeature.Commands;
using FluentValidation;

namespace Application.Features.TariffGridFeature.Validators
{
    public class UpdateTariffGridCommandValidator : ValidatorBase<UpdateTariffGridCommand>
    {
        public UpdateTariffGridCommandValidator()
        {
            RuleFor(v => v.Id)
                .NotEmpty().WithMessage("Grid ID is required");

            RuleFor(v => v.TariffGridDto.Name)
                .NotEmpty().WithMessage("Name is required")
                .MaximumLength(200).WithMessage("Name must not exceed 200 characters");

            RuleFor(v => v.TariffGridDto.TransportMode)
                .NotEmpty().WithMessage("Transport mode is required")
                .Must(IsValidTransportMode).WithMessage("Invalid transport mode");

            RuleFor(v => v.TariffGridDto.ValidFrom)
                .NotEmpty().WithMessage("Valid from date is required");

            RuleFor(v => v.TariffGridDto.CurrencyCode)
                .NotEmpty().WithMessage("Currency code is required")
                .Length(3).WithMessage("Currency code must be 3 characters");

            RuleFor(v => v.TariffGridDto.ValidTo)
                .Must((cmd, validTo) => !validTo.HasValue || validTo.Value > cmd.TariffGridDto.ValidFrom)
                .WithMessage("Valid to date must be after valid from date");
        }

        private static bool IsValidTransportMode(string transportMode)
        {
            return System.Enum.TryParse<Domain.Enums.TransportMode>(transportMode, true, out _);
        }
    }
}