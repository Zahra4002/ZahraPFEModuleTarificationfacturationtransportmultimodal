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
    public class AddInvoiceLineCommandHandler : IRequestHandler<AddInvoiceLineCommand, ResponseHttp>
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IInvoiceLineRepository _invoiceLineRepository;
        private readonly IMapper _mapper;

        public AddInvoiceLineCommandHandler(
            IInvoiceRepository invoiceRepository,
            IInvoiceLineRepository invoiceLineRepository,
            IMapper mapper)
        {
            _invoiceRepository = invoiceRepository;
            _invoiceLineRepository = invoiceLineRepository;
            _mapper = mapper;
        }

        public async Task<ResponseHttp> Handle(AddInvoiceLineCommand request, CancellationToken cancellationToken)
        {
            try
            {
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
                        Fail_Messages = $"Impossible d'ajouter une ligne à une facture avec le statut {invoice.Status}"
                    };
                }

                // 3️⃣ Créer la nouvelle ligne
                var line = new InvoiceLine
                {
                    Id = Guid.NewGuid(),
                    InvoiceId = invoice.Id,
                    Description = request.Description,
                    Quantity = request.Quantity,
                    UnitPriceHT = request.UnitPriceHT,
                    DiscountPercent = request.DiscountPercent,
                    VATRate = request.VatRate,
                    TransportSegmentId = request.TransportSegmentId,
                    // SurchargeId = request.SurchargeId, // Si tu as cette propriété
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = "System"
                };

                // 4️⃣ Ajouter la ligne
                await _invoiceLineRepository.AddAsync(line);
                await _invoiceLineRepository.SaveChangesAsync(cancellationToken);

                // 5️⃣ Recalculer les totaux de la facture
                await RecalculateInvoiceTotals(invoice, cancellationToken);

                // 6️⃣ Récupérer la facture mise à jour avec ses lignes
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