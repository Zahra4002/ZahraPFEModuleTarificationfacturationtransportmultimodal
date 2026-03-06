using Application.Common.Constants;
using Application.Common.Validator;
using Application.Features.InvoiceFeature.Commands;
using FluentValidation;

namespace Application.Features.InvoiceFeature.Validators
{
    public class AddInvoiceCommandValidator : ValidatorBase<AddInvoiceCommand>
    {
        public AddInvoiceCommandValidator()
        {
            RuleFor(v => v.InvoiceNumber)
                .NotEmpty()
                .WithMessage(ValidationConstants.InvoiceNumberMustHasValue);

          //  RuleFor(v => v.ClientId)
               // .NotEmpty()
               // .WithMessage(ValidationConstants.ClientIdMustHasValue);

            RuleFor(v => v.IssueDate)
                .NotEmpty()
                .WithMessage(ValidationConstants.IssueDateMustHasValue);

            RuleFor(v => v.DueDate)
                .NotEmpty()
                .WithMessage(ValidationConstants.DueDateMustHasValue);

            RuleFor(v => v.ExchangeRate)
                .GreaterThanOrEqualTo(1)
                .WithMessage("ExchangeRate doit être supérieur ou égal à 1");

            RuleFor(v => v.DueDate)
                .GreaterThan(v => v.IssueDate)
                .WithMessage("DueDate doit être supérieure à IssueDate");
        }
    }
}