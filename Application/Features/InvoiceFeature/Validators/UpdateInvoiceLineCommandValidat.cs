using Application.Common.Validator;
using Application.Features.InvoiceFeature.Commands;
using FluentValidation;

namespace Application.Features.InvoiceFeature.Validators
{
    public class UpdateInvoiceLineCommandValidator : ValidatorBase<UpdateInvoiceLineCommand>
    {
        public UpdateInvoiceLineCommandValidator()
        {
            RuleFor(v => v.InvoiceId)
                .NotEmpty()
                .WithMessage("L'ID de la facture est requis");

            RuleFor(v => v.LineId)
                .NotEmpty()
                .WithMessage("L'ID de la ligne est requis");

            RuleFor(v => v.Quantity)
                .GreaterThan(0)
                .WithMessage("La quantité doit être supérieure à 0");

            RuleFor(v => v.UnitPriceHT)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Le prix unitaire HT doit être supérieur ou égal à 0");

            RuleFor(v => v.VatRate)
                .InclusiveBetween(0, 100)
                .WithMessage("Le taux de TVA doit être entre 0 et 100");

            RuleFor(v => v.DiscountPercent)
                .InclusiveBetween(0, 100)
                .WithMessage("Le pourcentage de remise doit être entre 0 et 100");
        }
    }
}