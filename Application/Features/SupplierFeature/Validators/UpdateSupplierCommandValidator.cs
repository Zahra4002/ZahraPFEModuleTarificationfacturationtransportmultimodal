using Application.Common.Validator;
using Application.Features.SupplierFeature.Commands;
using FluentValidation;

namespace Application.Features.SupplierFeature.Validators
{
    public class UpdateSupplierCommandValidator : ValidatorBase<UpdateSupplierCommand>
    {
        public UpdateSupplierCommandValidator()
        {
            RuleFor(v => v.Id)
                .NotEmpty()
                .WithMessage("L'ID du fournisseur est requis");

            RuleFor(v => v.Email)
                .EmailAddress()
                .When(v => !string.IsNullOrEmpty(v.Email))
                .WithMessage("L'email n'est pas valide");

            RuleFor(v => v.DefaultCurrencyCode)
                .Length(3)
                .When(v => !string.IsNullOrEmpty(v.DefaultCurrencyCode))
                .WithMessage("Le code devise doit avoir 3 caractères");
        }
    }
}