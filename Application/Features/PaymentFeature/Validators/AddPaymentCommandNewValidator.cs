using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Validator;
using Application.Features.PaymentFeature.Commands;
using FluentValidation;

namespace Application.Features.PaymentFeature.Validators
{
    public class AddPaymentCommandNewValidator : ValidatorBase<AddPaymentCommandNew>
    {
        public AddPaymentCommandNewValidator()
        {
            // invoice – référence à la facture (souvent un code ou un ID)
            RuleFor(x => x.invoice)
                .NotEmpty().WithMessage("La référence de la facture est obligatoire.")
                .MaximumLength(50).WithMessage("La référence de la facture ne peut pas dépasser 50 caractères.")
                // .Matches(@"^[A-Z0-9\-/]+$").WithMessage("La référence facture doit contenir uniquement lettres majuscules, chiffres, tirets ou slash.")  // optionnel
                ;

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

            // paymentMethod – mode de paiement (ex: Virement, Carte, Espèces, Chèque...)
           

            // notes – optionnel
            RuleFor(x => x.notes)
                .MaximumLength(500).WithMessage("Les notes ne peuvent pas dépasser 500 caractères.")
                .When(x => !string.IsNullOrWhiteSpace(x.notes));
        }

        // Méthode d'aide pour valider les modes de paiement acceptés
        private bool BeAValidPaymentMethod(string paymentMethod)
        {
            var validMethods = new[]
            {
                "Virement", "Carte", "Espèces", "Chèque", "Prélèvement", "PayPal", "Autre"
            };  // ← adapte selon tes besoins réels

            return validMethods.Contains(paymentMethod, StringComparer.OrdinalIgnoreCase);
        }
    }
}
