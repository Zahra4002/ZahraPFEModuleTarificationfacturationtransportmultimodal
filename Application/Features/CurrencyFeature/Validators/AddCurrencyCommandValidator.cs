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
    public class AddCurrencyCommandValidator:ValidatorBase<AddCurrencyCommand>
    {
        public AddCurrencyCommandValidator()
        {
            RuleFor(x => x.Code).NotEmpty().WithMessage("Code is required.");
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required.");
            RuleFor(x => x.Symbol).NotEmpty().WithMessage("Symbol is required.");
            RuleFor(x => x.DecimalPlaces).GreaterThanOrEqualTo(0).WithMessage("Decimal Places must be a non-negative integer.");
        }
    }
}
    