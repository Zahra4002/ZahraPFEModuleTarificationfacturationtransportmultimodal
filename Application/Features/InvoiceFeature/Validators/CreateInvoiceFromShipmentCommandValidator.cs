using Application.Common.Validator;
using Application.Features.InvoiceFeature.Commands;
using FluentValidation;

namespace Application.Features.InvoiceFeature.Validators
{
    public class CreateInvoiceFromShipmentCommandValidator : ValidatorBase<CreateInvoiceFromShipmentCommand>
    {
        public CreateInvoiceFromShipmentCommandValidator()
        {
            RuleFor(v => v.ShipmentId)
                .NotEmpty()
                .WithMessage("ShipmentId est requis");

            // CurrencyId est optionnel, pas de validation requise
        }
    }
}