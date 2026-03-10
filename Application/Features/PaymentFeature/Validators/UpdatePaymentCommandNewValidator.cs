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
    public class UpdatePaymentCommandNewValidator : ValidatorBase<UpdatePaymentCommandNew>
    {
        public UpdatePaymentCommandNewValidator()
        {
            // paymentId – obligatoire pour identifier le paiement à modifier
            RuleFor(x => x.InvoiceId)
                .NotEmpty().WithMessage("L'identifiant du paiement est obligatoire.")
                .NotEqual(Guid.Empty).WithMessage("L'identifiant du paiement ne peut pas être vide.");

            // paymentDate – date du paiement
            RuleFor(x => x.paymentDate)
                .NotEmpty().WithMessage("La date de paiement est obligatoire.")
                .LessThanOrEqualTo(DateTime.UtcNow.AddDays(1)).WithMessage("La date de paiement ne peut pas être dans le futur.")
                .GreaterThan(DateTime.UtcNow.AddYears(-10)).WithMessage("La date de paiement semble trop ancienne.");

            // amount – montant payé
            RuleFor(x => x.amount)
                .GreaterThan(0).WithMessage("Le montant du paiement doit être supérieur à 0.")
                .PrecisionScale(18, 2, false).WithMessage("Le montant ne peut pas avoir plus de 2 décimales.")
                .LessThanOrEqualTo(999999999999m).WithMessage("Le montant semble excessivement élevé.");

          

            // notes – optionnel
            RuleFor(x => x.notes)
                .MaximumLength(1000).WithMessage("Les notes ne peuvent pas dépasser 1000 caractères.")
                .When(x => !string.IsNullOrWhiteSpace(x.notes));
        }

        // Méthode pour valider les modes de paiement acceptés
        private bool BeAValidPaymentMethod(string paymentMethod)
        {
            var validMethods = new[]
            {
                "Virement", "Carte", "Espèces", "Chèque", "Prélèvement", "PayPal", "Autre"
            };

            return validMethods.Contains(paymentMethod, StringComparer.OrdinalIgnoreCase);
        }

        // Méthode pour savoir quand la référence est obligatoire
        private bool NeedsReference(string paymentMethod)
        {
            var methodsRequiringRef = new[]
            {
                "Virement", "Chèque", "Carte"
            };

            return methodsRequiringRef.Contains(paymentMethod, StringComparer.OrdinalIgnoreCase);
        }
    }
}
