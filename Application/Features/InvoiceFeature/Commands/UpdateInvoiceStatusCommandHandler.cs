using Application.Features.InvoiceFeature.Dtos;
using Application.Interfaces;
using Application.Setting;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.InvoiceFeature.Commands
{
    public class UpdateInvoiceStatusCommandHandler : IRequestHandler<UpdateInvoiceStatusCommand, ResponseHttp>
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IMapper _mapper;

        public UpdateInvoiceStatusCommandHandler(
            IInvoiceRepository invoiceRepository,
            IMapper mapper)
        {
            _invoiceRepository = invoiceRepository;
            _mapper = mapper;
        }

        public async Task<ResponseHttp> Handle(UpdateInvoiceStatusCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // 1️⃣ Récupérer la facture
                var invoice = await _invoiceRepository.GetByIdAsync(request.Id);

                if (invoice == null)
                {
                    return new ResponseHttp
                    {
                        Status = StatusCodes.Status404NotFound,
                        Fail_Messages = $"Facture avec ID {request.Id} non trouvée"
                    };
                }

                // ✅ 2️⃣ Si le statut est le même, retourner directement sans erreur
                if (invoice.Status == request.Status)
                {
                    var invoiceDto = _mapper.Map<InvoiceDTO>(invoice);
                    return new ResponseHttp
                    {
                        Resultat = invoiceDto,
                        Status = StatusCodes.Status200OK
                    };
                }

                // 3️⃣ Vérifier la transition de statut valide
                if (!IsValidStatusTransition(invoice.Status, request.Status))
                {
                    return new ResponseHttp
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Fail_Messages = $"Transition de statut invalide de {invoice.Status} vers {request.Status}"
                    };
                }

                // 4️⃣ Mettre à jour le statut
                invoice.Status = request.Status;
                invoice.ModifiedDate = DateTime.UtcNow;
                invoice.ModifiedBy = "System";

                // 5️⃣ Sauvegarder
                await _invoiceRepository.UpdateAsync(invoice);
                await _invoiceRepository.SaveChangesAsync(cancellationToken);

                // 6️⃣ Retourner le DTO
                var updatedInvoiceDto = _mapper.Map<InvoiceDTO>(invoice);

                return new ResponseHttp
                {
                    Resultat = updatedInvoiceDto,
                    Status = StatusCodes.Status200OK
                };
            }
            catch (Exception ex)
            {
                return new ResponseHttp
                {
                    Fail_Messages = ex.Message,
                    Status = StatusCodes.Status400BadRequest
                };
            }
        }

        /// <summary>
        /// Vérifie si la transition de statut est valide
        /// </summary>
        private bool IsValidStatusTransition(InvoiceStatus currentStatus, InvoiceStatus newStatus)
        {
            // ✅ Si c'est le même statut, on autorise (géré avant, mais sécurité)
            if (currentStatus == newStatus)
                return true;

            // Règles de transition de statut
            return (currentStatus, newStatus) switch
            {
                // Brouillon → Émise ou Brouillon → Annulée
                (InvoiceStatus.Brouillon, InvoiceStatus.Emise) => true,
                (InvoiceStatus.Brouillon, InvoiceStatus.Annulee) => true,

                // Émise → Envoyée ou Émise → Annulée
                (InvoiceStatus.Emise, InvoiceStatus.Envoyee) => true,
                (InvoiceStatus.Emise, InvoiceStatus.Annulee) => true,

                // Envoyée → Payée ou Envoyée → PartiellementPayee
                (InvoiceStatus.Envoyee, InvoiceStatus.Payee) => true,
                (InvoiceStatus.Envoyee, InvoiceStatus.PartiellementPayee) => true,

                // PartiellementPayee → Payée
                (InvoiceStatus.PartiellementPayee, InvoiceStatus.Payee) => true,

                // Aucune autre transition n'est autorisée
                _ => false
            };
        }
    }
}