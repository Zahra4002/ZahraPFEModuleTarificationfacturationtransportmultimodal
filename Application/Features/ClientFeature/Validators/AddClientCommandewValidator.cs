using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Constants;
using Application.Common.Validator;
using Application.Features.ClientFeature.Commands;
using Application.Features.TestFeature.Commands;
using Domain.ValueObjects;
using FluentValidation;

namespace Application.Features.ClientFeature.Validators
{
    public class AddClientCommandNewValidator : ValidatorBase<AddClientCommandNew>
    {
        public AddClientCommandNewValidator()
        {
            // code – souvent obligatoire et avec format spécifique
            RuleFor(v => v.code)
                .NotEmpty().WithMessage("Le code client est obligatoire.")
                .MaximumLength(50).WithMessage("Le code client ne peut pas dépasser 50 caractères.")
                // .Matches(@"^[A-Z0-9\-]+$")  ← décommente si tu veux un format précis (lettres majuscules, chiffres, tiret)
                ;

            // name (nom / prénom / raison sociale personne)
            RuleFor(v => v.name)
                .NotEmpty()
                .WithMessage(ValidationConstants.FirstNameMustHasValue ?? "Le nom est obligatoire.")
                .MaximumLength(150).WithMessage("Le nom ne peut pas dépasser 150 caractères.");

            // taxId (matricule fiscal / identifiant TVA)
            RuleFor(v => v.taxId)
                .NotEmpty().WithMessage("Le matricule fiscal est obligatoire.")
                .MaximumLength(50).WithMessage("Le matricule fiscal ne peut pas dépasser 50 caractères.");

            // Email
            RuleFor(v => v.email)
                .NotEmpty().WithMessage("L'email est obligatoire.")
                .EmailAddress().WithMessage("L'adresse email n'est pas valide.")
                .MaximumLength(100);

            // phoneNumber
            RuleFor(v => v.phoneNumber)
                .NotEmpty().WithMessage("Le numéro de téléphone est obligatoire.")
                .Matches(@"^\+?[1-9][0-9]{7,14}$").WithMessage("Le format du téléphone n'est pas valide (ex: +216xxxxxxxx).")
                .MaximumLength(20);

            // userRole
            RuleFor(v => v.UserRole)
                .NotNull().WithMessage("Le rôle est obligatoire.")
                .IsInEnum().WithMessage("Le rôle sélectionné n'est pas valide.");

            // bullingAddress → obligatoire
            RuleFor(v => v.bullingAddress)
                .NotNull().WithMessage("L'adresse de facturation est obligatoire.")
                .SetValidator(new AddressValidator());

            // shippingAddress → optionnelle mais validée si présente
            RuleFor(v => v.shippingAddress)
                .SetValidator(new AddressValidator())
                .When(v => v.shippingAddress != null);

            // defaultCurrencyCode (ISO 4217)
            RuleFor(v => v.defaultCurrencyCode)
                .NotEmpty().WithMessage("La devise par défaut est obligatoire.")
                .Length(3).WithMessage("Le code devise doit contenir exactement 3 lettres.")
                .Matches(@"^[A-Z]{3}$").WithMessage("La devise doit être en majuscules (ex: TND, EUR, USD).");

            // creditLimit
            RuleFor(v => v.creditLimit)
                .GreaterThanOrEqualTo(0).WithMessage("La limite de crédit ne peut pas être négative.");

            // paymenttermDays
            RuleFor(v => v.paymenttermDays)
                .GreaterThanOrEqualTo(0).WithMessage("Les jours de paiement ne peuvent pas être négatifs.")
                .LessThanOrEqualTo(180).WithMessage("Les jours de paiement ne devraient pas dépasser 180 jours.");

            // isActive – pas besoin de validation forte (booléen par défaut)
        }
    }

    // Validator pour Address (Value Object) – à mettre dans le même fichier ou dans un fichier séparé
    public class AddressValidator : AbstractValidator<Address>
    {
        public AddressValidator()
        {
            RuleFor(a => a.Street)
                .NotEmpty().WithMessage("La rue est obligatoire.")
                .MaximumLength(200);

            RuleFor(a => a.City)
                .NotEmpty().WithMessage("La ville est obligatoire.")
                .MaximumLength(100);

            RuleFor(a => a.PostalCode)
                .NotEmpty().WithMessage("Le code postal est obligatoire.")
                .MaximumLength(20);

            RuleFor(a => a.Country)
                .NotEmpty().WithMessage("Le pays est obligatoire.")
                .MaximumLength(100);
        }
    }
}