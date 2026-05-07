using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Validator;
using Application.Features.PaymentFeature.Commands;
using Domain.Enums;  // ✅ AJOUTER
using FluentValidation;

namespace Application.Features.PaymentFeature.Validators
{
    public class AddPaymentCommandNewValidator : ValidatorBase<AddPaymentCommandNew>
    {
        public AddPaymentCommandNewValidator()
        {
            // ✅ CORRECTION: invoiceId – OBLIGATOIRE si facture doit être liée
            RuleFor(x => x.invoiceId)
                .NotEmpty().WithMessage("L'ID de la facture est obligatoire.")
                .Must(id => id != Guid.Empty).WithMessage("L'ID de la facture doit être valide.");

            // paymentDate – date du paiement
            RuleFor(x => x.paymentDate)
                .NotEmpty().WithMessage("La date de paiement est obligatoire.")
                .LessThanOrEqualTo(DateTime.UtcNow.AddDays(1)).WithMessage("La date de paiement ne peut pas être dans le futur.")
                .GreaterThan(DateTime.UtcNow.AddYears(-5)).WithMessage("La date de paiement semble trop ancienne.");

            // amount – montant payé
            RuleFor(x => x.amount)
                .NotEmpty().WithMessage("Le montant du paiement est obligatoire.")
                .GreaterThan(0).WithMessage("Le montant du paiement doit être supérieur à 0.")
                .PrecisionScale(18, 2, false).WithMessage("Le montant ne peut pas avoir plus de 2 décimales.")
                .LessThanOrEqualTo(999999999999m).WithMessage("Le montant semble excessivement élevé (limite technique).");

            // ✅ CORRECTION: paymentMethod – mode de paiement OBLIGATOIRE
            RuleFor(x => x.paymentMethod)
                .NotEmpty().WithMessage("Le mode de paiement est obligatoire.")
                .Must(BeAValidPaymentMethod).WithMessage("Le mode de paiement est invalide.");

            // reference – optionnel
            RuleFor(x => x.reference)
                .MaximumLength(100).WithMessage("La référence ne peut pas dépasser 100 caractères.")
                .When(x => !string.IsNullOrWhiteSpace(x.reference));

            // notes – optionnel
            RuleFor(x => x.notes)
                .MaximumLength(500).WithMessage("Les notes ne peuvent pas dépasser 500 caractères.")
                .When(x => !string.IsNullOrWhiteSpace(x.notes));
        }

        // ✅ CORRECTION: Méthode pour valider les modes de paiement
        private bool BeAValidPaymentMethod(PaymentMethod paymentMethod)
        {
            // Les valeurs enum valides sont: 1, 2, 3, 4, 5
            return Enum.IsDefined(typeof(PaymentMethod), paymentMethod);
        }
    }
}
