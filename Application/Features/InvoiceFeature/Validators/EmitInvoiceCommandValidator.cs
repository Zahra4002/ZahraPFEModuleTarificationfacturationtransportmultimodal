using Application.Common.Validator;
using Application.Features.InvoiceFeature.Commands;
using FluentValidation;

namespace Application.Features.InvoiceFeature.Validators
{
    public class EmitInvoiceCommandValidator : ValidatorBase<EmitInvoiceCommand>
    {
        public EmitInvoiceCommandValidator()
        {
            RuleFor(v => v.Id)
                .NotEmpty()
                .WithMessage("L'ID de la facture est requis");
        }
    }
}