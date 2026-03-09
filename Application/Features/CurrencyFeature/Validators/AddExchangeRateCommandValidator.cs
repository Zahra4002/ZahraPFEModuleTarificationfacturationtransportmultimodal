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
    public class AddExchangeRateCommandValidator : ValidatorBase<AddExchangeRateCommand>
    {

        public AddExchangeRateCommandValidator()
        {
            RuleFor(x => x.fromCurrencyId).NotEmpty().WithMessage("FromCurrencyId is require");
            RuleFor(x => x.toCurrencyId).NotEmpty().WithMessage("ToCurrencyId");
            RuleFor(x => x.rate).GreaterThan(0).WithMessage("Rate must be greater than 0");
            RuleFor(x => x.effectiveDate).LessThanOrEqualTo(DateTime.UtcNow).WithMessage("EffectiveDate must be in the past or present");
            RuleFor(x => x.expiryDate).GreaterThan(x => x.effectiveDate).WithMessage("ExpiryDate must be greater than EffectiveDate");

        }
    }
}
