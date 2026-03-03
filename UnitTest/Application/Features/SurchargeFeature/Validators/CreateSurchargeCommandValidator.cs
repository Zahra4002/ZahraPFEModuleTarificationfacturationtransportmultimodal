// Application/Features/SurchargeFeature/Validators/CreateSurchargeCommandValidator.cs
using Application.Common.Validator;
using Application.Features.SurchargeFeature.Commands;
using FluentValidation;

namespace Application.Features.SurchargeFeature.Validators
{
    public class CreateSurchargeCommandValidator : ValidatorBase<CreateSurchargeCommand>
    {
        public CreateSurchargeCommandValidator()
        {
            RuleFor(v => v.SurchargeDto.Code)
                .NotEmpty().WithMessage("Code is required")
                .MaximumLength(50).WithMessage("Code must not exceed 50 characters");

            RuleFor(v => v.SurchargeDto.Name)
                .NotEmpty().WithMessage("Name is required")
                .MaximumLength(200).WithMessage("Name must not exceed 200 characters");

            RuleFor(v => v.SurchargeDto.Type)
                .NotEmpty().WithMessage("Type is required")
                .Must(type => new[] { "Carburant", "CongestionPortuaire", "Securite", "Saisonnalite", "MatieresDangereuses", "HorsGabarit" }
                    .Contains(type, StringComparer.OrdinalIgnoreCase))
                .WithMessage("Invalid surcharge type");

            RuleFor(v => v.SurchargeDto.CalculationType)
                .NotEmpty().WithMessage("Calculation type is required")
                .Must(calcType => new[] { "PerLine", "PerInvoice", "Custom" }
                    .Contains(calcType, StringComparer.OrdinalIgnoreCase))
                .WithMessage("Invalid calculation type");

            RuleFor(v => v.SurchargeDto.Value)
                .GreaterThanOrEqualTo(0).WithMessage("Value must be greater than or equal to 0");
        }
    }
}