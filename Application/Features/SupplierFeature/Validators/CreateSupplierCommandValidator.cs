using Application.Common.Validator;
using Application.Features.SupplierFeature.Commands;
using FluentValidation;

namespace Application.Features.SupplierFeature.Validators
{
    public class CreateSupplierCommandValidator : ValidatorBase<CreateSupplierCommand>
    {
        public CreateSupplierCommandValidator()
        {
            RuleFor(v => v.Code)
                .NotEmpty()
                .WithMessage("Le code du fournisseur est requis")
                .MaximumLength(50)
                .WithMessage("Le code ne peut pas dépasser 50 caractères");

            RuleFor(v => v.Name)
                .NotEmpty()
                .WithMessage("Le nom du fournisseur est requis")
                .MaximumLength(200)
                .WithMessage("Le nom ne peut pas dépasser 200 caractères");

            RuleFor(v => v.Email)
                .EmailAddress()
                .When(v => !string.IsNullOrEmpty(v.Email))
                .WithMessage("L'email n'est pas valide");

            RuleFor(v => v.DefaultCurrencyCode)
                .NotEmpty()
                .WithMessage("La devise par défaut est requise")
                .Length(3)
                .WithMessage("Le code devise doit avoir 3 caractères");
        }
    }
}