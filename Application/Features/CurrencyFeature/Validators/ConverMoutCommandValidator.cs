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
    public class ConverMoutCommandValidator : ValidatorBase<ConverMoutCommand>
    {
        public ConverMoutCommandValidator()
        {
            RuleFor(x => x.FromCurrencyCode).NotEmpty().WithMessage("From Currency Code is required.");
            RuleFor(x => x.ToCurrencyCode).NotEmpty().WithMessage("To Currency Code is required.");
            RuleFor(x => x.Amount).GreaterThan(0).WithMessage("Amount must be greater than zero.");
            RuleFor(x => x.Date).LessThanOrEqualTo(DateTime.Now).WithMessage("Date cannot be in the future.");
        }

    }
}
