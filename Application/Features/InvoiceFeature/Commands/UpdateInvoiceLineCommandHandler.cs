using Application.Features.InvoiceFeature.Dtos;
using Application.Interfaces;
using Application.Setting;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.InvoiceFeature.Commands
{
    public class UpdateInvoiceLineCommandHandler : IRequestHandler<UpdateInvoiceLineCommand, ResponseHttp>
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IInvoiceLineRepository _invoiceLineRepository;
        private readonly IMapper _mapper;

        public UpdateInvoiceLineCommandHandler(
            IInvoiceRepository invoiceRepository,
            IInvoiceLineRepository invoiceLineRepository,
            IMapper mapper)
        {
            _invoiceRepository = invoiceRepository;
            _invoiceLineRepository = invoiceLineRepository;
            _mapper = mapper;
        }

        public async Task<ResponseHttp> Handle(UpdateInvoiceLineCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // ✅ Vérifier que l'ID de la ligne n'est pas vide
                if (request.LineId == Guid.Empty)
                {
                    return new ResponseHttp
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Fail_Messages = "L'ID de la ligne ne peut pas être vide"
                    };
                }
                // 1️⃣ Récupérer la facture
                var invoice = await _invoiceRepository.GetByIdAsync(request.InvoiceId);

                if (invoice == null)
                {
                    return new ResponseHttp
                    {
                        Status = StatusCodes.Status404NotFound,
                        Fail_Messages = $"Facture avec ID {request.InvoiceId} non trouvée"
                    };
                }

                // 2️⃣ Vérifier que la facture est modifiable
                if (invoice.Status != InvoiceStatus.Brouillon)
                {
                    return new ResponseHttp
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Fail_Messages = $"Impossible de modifier une ligne sur une facture avec le statut {invoice.Status}"
                    };
                }

                // 3️⃣ Récupérer la ligne à modifier
                var line = await _invoiceLineRepository.GetByIdAsync(request.LineId);

                if (line == null)
                {
                    return new ResponseHttp
                    {
                        Status = StatusCodes.Status404NotFound,
                        Fail_Messages = $"Ligne avec ID {request.LineId} non trouvée"
                    };
                }

                // 4️⃣ Vérifier que la ligne appartient bien à la facture
                if (line.InvoiceId != request.InvoiceId)
                {
                    return new ResponseHttp
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Fail_Messages = "Cette ligne n'appartient pas à la facture spécifiée"
                    };
                }

                // 5️⃣ Mettre à jour la ligne
                line.Description = request.Description;
                line.Quantity = request.Quantity;
                line.UnitPriceHT = request.UnitPriceHT;
                line.DiscountPercent = request.DiscountPercent;
                line.VATRate = request.VatRate;
                line.ModifiedDate = DateTime.UtcNow;
                line.ModifiedBy = "System";

                // 6️⃣ Sauvegarder la ligne modifiée
                await _invoiceLineRepository.UpdateAsync(line);
                await _invoiceLineRepository.SaveChangesAsync(cancellationToken);

                // 7️⃣ Recalculer les totaux de la facture
                await RecalculateInvoiceTotals(invoice, cancellationToken);

                // 8️⃣ Récupérer la facture mise à jour
                var updatedInvoice = await _invoiceRepository.GetByIdWithDetailsAsync(invoice.Id);
                var invoiceDto = _mapper.Map<InvoiceDTO>(updatedInvoice);

                return new ResponseHttp
                {
                    Resultat = invoiceDto,
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

        private async Task RecalculateInvoiceTotals(Invoice invoice, CancellationToken cancellationToken)
        {
            // Recharger la facture avec ses lignes
            var invoiceWithLines = await _invoiceRepository.GetByIdWithDetailsAsync(invoice.Id);

            if (invoiceWithLines != null && invoiceWithLines.Lines != null)
            {
                invoiceWithLines.TotalHT = invoiceWithLines.Lines.Sum(l => l.Quantity * l.UnitPriceHT);
                invoiceWithLines.TotalVAT = invoiceWithLines.Lines.Sum(l => l.Quantity * l.UnitPriceHT * (l.VATRate / 100));
                invoiceWithLines.TotalTTC = invoiceWithLines.TotalHT + invoiceWithLines.TotalVAT;
                invoiceWithLines.ModifiedDate = DateTime.UtcNow;
                invoiceWithLines.ModifiedBy = "System";

                await _invoiceRepository.UpdateAsync(invoiceWithLines);
                await _invoiceRepository.SaveChangesAsync(cancellationToken);
            }
        }
    }
}