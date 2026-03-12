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
    public class UpdateZoneValidator : ValidatorBase<UpdateZoneCommand>
    {
   
            public UpdateZoneValidator()
        {
            RuleFor(v => v.Id)
                .NotEmpty()
                .WithMessage("Zone ID must be provided.");
        }
    }
}
