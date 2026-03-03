using Application.Common.Validator;
using Application.Features.SurchargeFeature.Commands;
using FluentValidation;

namespace Application.Features.SurchargeFeature.Validators
{
    public class CreateSurchargeRuleCommandValidator : ValidatorBase<CreateSurchargeRuleCommand>
    {
        public CreateSurchargeRuleCommandValidator()
        {
            RuleFor(v => v.SurchargeId)
                .NotEmpty().WithMessage("Surcharge ID is required");

            RuleFor(v => v.RuleDto.Name)
                .NotEmpty().WithMessage("Rule name is required")
                .MaximumLength(200).WithMessage("Rule name must not exceed 200 characters");

            // CORRECTION : Utiliser 'cmd' au lieu de 'dto' pour éviter la confusion
            RuleFor(v => v.RuleDto.ValidTo)
                .Must((cmd, validTo) => !validTo.HasValue || !cmd.RuleDto.ValidFrom.HasValue || validTo.Value > cmd.RuleDto.ValidFrom.Value)
                .WithMessage("Valid to date must be after valid from date");

            RuleFor(v => v.RuleDto.Priority)
                .GreaterThanOrEqualTo(0).WithMessage("Priority must be greater than or equal to 0");
        }
    }
}