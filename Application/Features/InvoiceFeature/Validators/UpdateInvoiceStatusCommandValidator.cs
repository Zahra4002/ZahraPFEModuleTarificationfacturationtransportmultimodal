using Application.Common.Validator;
using Application.Features.InvoiceFeature.Commands;
using FluentValidation;

namespace Application.Features.InvoiceFeature.Validators
{
    public class UpdateInvoiceStatusCommandValidator : ValidatorBase<UpdateInvoiceStatusCommand>
    {
        public UpdateInvoiceStatusCommandValidator()
        {
            RuleFor(v => v.Id)
                .NotEmpty()
                .WithMessage("L'ID de la facture est requis");

            RuleFor(v => v.Status)
                .IsInEnum()
                .WithMessage("Statut invalide");
        }
    }
}