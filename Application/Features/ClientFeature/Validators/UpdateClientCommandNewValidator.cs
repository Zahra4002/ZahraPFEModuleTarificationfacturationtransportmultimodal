using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Constants;
using Application.Common.Validator;
using Application.Features.ClientFeature.Commands;
using FluentValidation;

namespace Application.Features.ClientFeature.Validators
{
    public class UpdateClientCommandNewValidator : ValidatorBase<UpdateClientCommandNew>
    {
        public UpdateClientCommandNewValidator()
        {
            // ClientId – obligatoire pour l'update
            RuleFor(x => x.ClientId)
                .NotEmpty().WithMessage("L'identifiant du client est obligatoire.")
                .NotEqual(Guid.Empty).WithMessage("L'identifiant du client ne peut pas être vide.");

            // code – souvent obligatoire et unique
            RuleFor(x => x.code)
                .NotEmpty().WithMessage("Le code client est obligatoire.")
                .MaximumLength(50).WithMessage("Le code client ne peut pas dépasser 50 caractères.")
                // .Matches(@"^[A-Z0-9\-]+$").WithMessage("Le code doit contenir uniquement lettres majuscules, chiffres et tirets.")  // décommente si besoin
                ;

            // name
            RuleFor(x => x.name)
                .NotEmpty().WithMessage(ValidationConstants.FirstNameMustHasValue ?? "Le nom est obligatoire.")
                .MaximumLength(150).WithMessage("Le nom ne peut pas dépasser 150 caractères.");

            // taxId (optionnel ou obligatoire selon ton business – ici je le mets obligatoire)
            RuleFor(x => x.taxId)
                .NotEmpty().WithMessage("Le matricule fiscal / identifiant TVA est obligatoire.")
                .MaximumLength(50).WithMessage("Le matricule fiscal ne peut pas dépasser 50 caractères.");

            // email
            RuleFor(x => x.email)
                .NotEmpty().WithMessage("L'email est obligatoire.")
                .EmailAddress().WithMessage("L'adresse email n'est pas valide.")
                .MaximumLength(100).WithMessage("L'email ne peut pas dépasser 100 caractères.");

            // phoneNumber
            RuleFor(x => x.phoneNumber)
                .NotEmpty().WithMessage("Le numéro de téléphone est obligatoire.")
                .Matches(@"^\+?[1-9]\d{7,14}$").WithMessage("Le format du numéro de téléphone n'est pas valide (ex: +216xxxxxxxx).")
                .MaximumLength(20);

            // userRole
            RuleFor(x => x.userRole)
                .NotNull().WithMessage("Le rôle est obligatoire.")
                .IsInEnum().WithMessage("Le rôle sélectionné n'est pas valide.");

            // bullingAddress → obligatoire (à corriger en billingAddress plus tard)
            RuleFor(x => x.bullingAddress)
                .NotNull().WithMessage("L'adresse de facturation est obligatoire.")
                .SetValidator(new AddressValidator());

            // shippingAddress → optionnelle
            RuleFor(x => x.shippingAddress)
                .SetValidator(new AddressValidator())
                .When(x => x.shippingAddress != null);

            // defaultCurrencyCode
            RuleFor(x => x.defaultCurrencyCode)
                .NotEmpty().WithMessage("La devise par défaut est obligatoire.")
                .Length(3).WithMessage("Le code devise doit être exactement 3 lettres (ISO 4217).")
                .Matches(@"^[A-Z]{3}$").WithMessage("Le code devise doit être en majuscules (ex: TND, EUR, USD).");

            // creditLimit
            RuleFor(x => x.creditLimit)
                .GreaterThanOrEqualTo(0).WithMessage("La limite de crédit ne peut pas être négative.");

            // paymenttermDays
            RuleFor(x => x.paymenttermDays)
                .GreaterThanOrEqualTo(0).WithMessage("Les jours de terme de paiement ne peuvent pas être négatifs.")
                .LessThanOrEqualTo(180).WithMessage("Le terme de paiement ne devrait pas dépasser 180 jours.");

            // isActive – pas de validation forte nécessaire (booléen)
        }
    }
}