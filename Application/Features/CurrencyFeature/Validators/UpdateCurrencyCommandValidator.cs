using Application.Common.Validator;
using Application.Features.CurrencyFeature.Commands;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.CurrencyFeature.Validators
{
    public class UpdateCurrencyCommandValidator : ValidatorBase<UpdateCurrencyCommand>
    {
        public UpdateCurrencyCommandValidator() { 
           RuleFor(x => x.Id).NotEmpty().WithMessage("Currency Id is required.");
        }
    }
}
