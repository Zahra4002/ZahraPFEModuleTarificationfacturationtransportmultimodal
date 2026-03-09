using Application.Common.Validator;
using Application.Features.ZoneFeature.Commands;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.ZoneFeature.Validators
{
    public class AddZoneCommandValidator : ValidatorBase<AddZoneCommand>
    {
        public AddZoneCommandValidator()
        {
            RuleFor(v => v.Code)
                .NotEmpty()
                .WithMessage("Code must have a value.");

            RuleFor(v => v.Name)
                .NotEmpty()
                .WithMessage("Name must have a value.");

            RuleFor(v => v.Country)
                .NotEmpty()
                .WithMessage("Country must have a value.");

            RuleFor(v => v.Region)
                .MaximumLength(100)
                .WithMessage("Region must not exceed 100 characters.")
                .When(v => v.Region != null);

            RuleFor(v => v.Description)
                .MaximumLength(250)
                .WithMessage("Description must not exceed 250 characters.")
                .When(v => v.Description != null);

            RuleFor(v => v.IsActive)
                .NotNull()
                .WithMessage("IsActive must be specified.");
        }

    }
}
