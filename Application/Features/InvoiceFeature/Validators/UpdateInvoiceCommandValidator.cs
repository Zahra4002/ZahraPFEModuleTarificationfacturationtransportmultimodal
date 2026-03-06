using Application.Common.Constants;
using Application.Common.Validator;
using Application.Features.InvoiceFeature.Commands;
using FluentValidation;

namespace Application.Features.InvoiceFeature.Validators
{
    public class UpdateInvoiceCommandValidator
        : ValidatorBase<UpdateInvoiceCommand>
    {
        public UpdateInvoiceCommandValidator()
        {
            RuleFor(v => v.invoiceId)
                .NotEmpty()
                .WithMessage(ValidationConstants.IdMustHasValue);

            RuleFor(v => v.invoiceNumber)
                .NotEmpty()
                .WithMessage(ValidationConstants.InvoiceNumberMustHasValue);

          //  RuleFor(v => v.clientId)
           //     .NotEmpty()
               // .WithMessage(ValidationConstants.ClientIdMustHasValue);

            RuleFor(v => v.issueDate)
                .NotEmpty()
                .WithMessage(ValidationConstants.IssueDateMustHasValue);

            RuleFor(v => v.dueDate)
                .NotEmpty()
                .WithMessage(ValidationConstants.DueDateMustHasValue);

            RuleFor(v => v.exchangeRate)
                .GreaterThan(0)
                .WithMessage(ValidationConstants.ExchangeRateMustBeGreaterThanZero);
        }
    }
}