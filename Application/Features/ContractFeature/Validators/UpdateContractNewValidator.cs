using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Validator;
using Application.Features.ContractFeature.Commands;
using FluentValidation;

namespace Application.Features.ContractFeature.Validators
{
    public class UpdateContractCommandNewValidator : ValidatorBase<UpdateContractCommandNew>
    {
        public UpdateContractCommandNewValidator()
        {
            // ContractId (obligatoire pour la mise à jour)
            RuleFor(x => x.ContractId)
                .NotEmpty().WithMessage("L'ID du contrat est obligatoire pour la mise à jour")
                .NotEqual(Guid.Empty).WithMessage("L'ID du contrat est invalide");

            // ContractNumber (optionnel mais si fourni → contraintes)
            RuleFor(x => x.contractNumber)
                .MaximumLength(50).When(x => !string.IsNullOrWhiteSpace(x.contractNumber))
                .WithMessage("Le numéro de contrat ne doit pas dépasser 50 caractères")
                .Matches(@"^[A-Za-z0-9\-\/]+$").When(x => !string.IsNullOrWhiteSpace(x.contractNumber))
                .WithMessage("Le numéro de contrat contient des caractères non autorisés");

            // Name (optionnel mais si fourni → contraintes)
            RuleFor(x => x.name)
                .MaximumLength(200).When(x => !string.IsNullOrWhiteSpace(x.name))
                .WithMessage("Le nom ne doit pas dépasser 200 caractères");

            // Type (optionnel, mais si fourni → doit être valide)
            RuleFor(x => x.Type)
                .IsInEnum().When(x => x.Type != default)
                .WithMessage("Type de contrat invalide");

            // ValidFrom et ValidTo (cohérence si les deux sont fournis)
            When(x => x.validForm != default || x.validTo != default, () =>
            {
                RuleFor(x => x.validForm)
                    .LessThan(x => x.validTo)
                    .When(x => x.validTo != default)
                    .WithMessage("La date de début doit être antérieure à la date de fin");

                RuleFor(x => x.validTo)
                    .GreaterThan(x => x.validForm)
                    .When(x => x.validForm != default)
                    .WithMessage("La date de fin doit être postérieure à la date de début");
            });

            // Au moins un client ou fournisseur (si les deux sont vides → erreur)
            RuleFor(x => x)
                .Must(x => x.ClientId != Guid.Empty || x.SupplierId != Guid.Empty)
                .WithMessage("Il faut conserver au moins un client ou un fournisseur (ClientId ou SupplierId)");

            // Terms et TermsAccepted (cohérence)
            RuleFor(x => x.termsAccepted)
                .Equal(true)
                .When(x => !string.IsNullOrWhiteSpace(x.terms))
                .WithMessage("Les conditions générales doivent être acceptées si des termes sont modifiés");

            // MinimumVolume (si fourni)
            RuleFor(x => x.minimumVolume)
                .GreaterThanOrEqualTo(0)
                .When(x => x.minimumVolume != default)
                .WithMessage("Le volume minimum engagé doit être positif ou zéro");

            // MinimumVolumeUnit (obligatoire si MinimumVolume > 0)
            RuleFor(x => x.minimumVolumeUnit)
                .NotEmpty()
                .When(x => x.minimumVolume > 0)
                .WithMessage("L'unité du volume minimum est obligatoire si un volume est engagé");

            // AutoRenew et RenewalNoticeDays
            RuleFor(x => x.RenewalNoticeDays)
                .GreaterThan(0)
                .When(x => x.AutoRenew)
                .WithMessage("Le délai de préavis de renouvellement doit être positif si le renouvellement automatique est activé");
        }
    }
}
