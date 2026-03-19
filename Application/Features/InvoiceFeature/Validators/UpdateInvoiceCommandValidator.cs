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
            // ✅ PascalCase - correspond au record
            RuleFor(v => v.InvoiceId)
                .NotEmpty()
                .WithMessage(ValidationConstants.IdMustHasValue);

            RuleFor(v => v.InvoiceNumber)
                .NotEmpty()
                .WithMessage(ValidationConstants.InvoiceNumberMustHasValue)
                .MaximumLength(50)
                .WithMessage("Invoice number cannot exceed 50 characters");

            RuleFor(v => v.SupplierId)
                .NotEmpty()
                .WithMessage("Supplier ID is required");

            RuleFor(v => v.ClientId)
                .NotEmpty()
                .WithMessage(ValidationConstants.ClientIdMustHasValue);

            RuleFor(v => v.IssueDate)
                .NotEmpty()
                .WithMessage(ValidationConstants.IssueDateMustHasValue)
                .LessThanOrEqualTo(v => v.DueDate)
                .WithMessage("Issue date must be before or equal to due date");

            RuleFor(v => v.DueDate)
                .NotEmpty()
                .WithMessage(ValidationConstants.DueDateMustHasValue)
                .GreaterThan(v => v.IssueDate)
                .WithMessage("Due date must be after issue date");

            RuleFor(v => v.CurrencyId)
                .NotEmpty()
                .WithMessage("Currency is required");

            RuleFor(v => v.ExchangeRate)
                .GreaterThan(0)
                .WithMessage(ValidationConstants.ExchangeRateMustBeGreaterThanZero);

            RuleFor(v => v.Notes)
                .MaximumLength(500)
                .WithMessage("Notes cannot exceed 500 characters")
                .When(v => !string.IsNullOrEmpty(v.Notes));
        }
    }
}