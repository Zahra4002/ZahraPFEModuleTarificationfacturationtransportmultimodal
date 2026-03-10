using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.ContractFeature.Validators
{
    using FluentValidation;
    using Domain.Enums;
    using global::Application.Features.ContractFeature.Commands;
    using global::Application.Common.Validator;

    namespace Application.Features.ContractFeature.Validators
    {
        public class AddContractCommandNewValidator : ValidatorBase<AddContractCommandNew>
        {
            public AddContractCommandNewValidator()
            {
                // ContractNumber
                RuleFor(x => x.contractNumber)
                    .NotEmpty().WithMessage("Le numéro de contrat est obligatoire")
                    .MaximumLength(50).WithMessage("Le numéro de contrat ne doit pas dépasser 50 caractères")
                    .Matches(@"^[A-Za-z0-9\-\/]+$").WithMessage("Le numéro de contrat contient des caractères non autorisés");

                // Name
                RuleFor(x => x.name)
                    .NotEmpty().WithMessage("Le nom du contrat est obligatoire")
                    .MaximumLength(200).WithMessage("Le nom ne doit pas dépasser 200 caractères");

                // Type (enum)
                RuleFor(x => x.Type)
                    .NotNull().WithMessage("Le type de contrat est obligatoire")
                    .IsInEnum().WithMessage("Type de contrat invalide");

                // Période de validité
                RuleFor(x => x.validForm)
                    .NotEmpty().WithMessage("La date de début (ValidFrom) est obligatoire");

                RuleFor(x => x.validTo)
                    .NotEmpty().WithMessage("La date de fin (ValidTo) est obligatoire");

                RuleFor(x => x)
                    .Must(x => x.validForm < x.validTo)
                    .WithMessage("La date de début doit être antérieure à la date de fin");

                // Au moins un client ou fournisseur
                RuleFor(x => x)
                    .Must(x => x.ClientId != Guid.Empty || x.SupplierId != Guid.Empty)
                    .WithMessage("Il faut spécifier au moins un client ou un fournisseur (ClientId ou SupplierId)");

                // Terms et TermsAccepted
                RuleFor(x => x.termsAccepted)
                    .Equal(true)
                    .When(x => !string.IsNullOrWhiteSpace(x.terms))
                    .WithMessage("Les conditions générales doivent être acceptées si des termes sont fournis");

                // MinimumVolume (si renseigné)
               

                // MinimumVolumeUnit (si minimumVolume > 0)
                RuleFor(x => x.minimumVolumeUnit)
                    .NotEmpty()
                    .When(x => x.minimumVolume > 0)
                    .WithMessage("L'unité du volume minimum est obligatoire si un volume est engagé");

                // AutoRenew et RenewalNoticeDays
                RuleFor(x => x.RenewalNoticeDays)
                    .GreaterThan(0)
                    .When(x => x.AutoRenew)
                    .WithMessage("Le délai de préavis de renouvellement doit être positif si le renouvellement automatique est activé");

                // GlobalDiscountPercent (si tu l'ajoutes plus tard)
                // RuleFor(x => x.GlobalDiscountPercent)
                //     .InclusiveBetween(0, 100)
                //     .WithMessage("La remise globale doit être entre 0% et 100%");
            }
        }
    }
}
