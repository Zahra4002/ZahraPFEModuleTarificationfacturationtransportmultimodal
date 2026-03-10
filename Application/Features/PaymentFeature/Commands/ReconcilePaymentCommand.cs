using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces;
using Application.Setting;
using MediatR;

namespace Application.Features.PaymentFeature.Commands
{
    public record ReconcilePaymentCommand(Guid PaymentId) : IRequest<ResponseHttp>;

    // Le Handler
    public class ReconcilePaymentCommandHandler : IRequestHandler<ReconcilePaymentCommand, ResponseHttp>
    {
        private readonly IPaymentRepository _paymentRepository;

        public ReconcilePaymentCommandHandler(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }

        public async Task<ResponseHttp> Handle(ReconcilePaymentCommand request, CancellationToken ct)
        {
            try
            {
                // Récupérer le paiement
                var payment = await _paymentRepository.GetByIdAsync(request.PaymentId, ct);

                if (payment == null)
                {
                    return new ResponseHttp
                    {
                        Status = 404,
                        Fail_Messages = "Paiement non trouvé"
                    };
                }

                // Vérifier si déjà réconcilié (recherche du tag dans Notes)
                const string RECONCILE_TAG = "[RECONCILED:";
                if (payment.Notes?.Contains(RECONCILE_TAG) == true)
                {
                    return new ResponseHttp
                    {
                        Status = 400,
                        Fail_Messages = "Ce paiement est déjà réconcilié"
                    };
                }

                // Marquer comme réconcilié en ajoutant le tag dans Notes
                string timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm");
                string newNote = $"[RECONCILED:{timestamp}]";

                // Ajouter le tag (sans écraser les notes existantes)
                payment.Notes = string.IsNullOrWhiteSpace(payment.Notes)
                    ? newNote
                    : $"{payment.Notes} {newNote}";

                // Mettre à jour et sauvegarder
                await _paymentRepository.UpdateAsync(payment, ct);
                await _paymentRepository.SaveChangesAsync(ct);

                return new ResponseHttp
                {
                    Status = 200,
                    Resultat = $"Paiement réconcilié avec succès le {timestamp}",
                    Fail_Messages = null
                };
            }
            catch (Exception ex)
            {
                return new ResponseHttp
                {
                    Status = 500,
                    Fail_Messages = $"Erreur lors de la réconciliation : {ex.Message}"
                };
            }
        }
    }
}
