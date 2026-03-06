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
    public class RecalculateInvoiceCommandHandler : IRequestHandler<RecalculateInvoiceCommand, ResponseHttp>
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IMapper _mapper;

        public RecalculateInvoiceCommandHandler(
            IInvoiceRepository invoiceRepository,
            IMapper mapper)
        {
            _invoiceRepository = invoiceRepository;
            _mapper = mapper;
        }

        public async Task<ResponseHttp> Handle(RecalculateInvoiceCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // 1️⃣ Récupérer la facture avec ses lignes
                var invoice = await _invoiceRepository.GetByIdWithDetailsAsync(request.Id);

                if (invoice == null)
                {
                    return new ResponseHttp
                    {
                        Status = StatusCodes.Status404NotFound,
                        Fail_Messages = $"Facture avec ID {request.Id} non trouvée"
                    };
                }

                // 2️⃣ Vérifier que la facture est modifiable (optionnel)
                if (invoice.Status != InvoiceStatus.Brouillon)
                {
                    return new ResponseHttp
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Fail_Messages = $"Impossible de recalculer une facture avec le statut {invoice.Status}"
                    };
                }

                // 3️⃣ Sauvegarder les anciennes valeurs pour log (optionnel)
                var oldTotalHT = invoice.TotalHT;
                var oldTotalVAT = invoice.TotalVAT;
                var oldTotalTTC = invoice.TotalTTC;

                // 4️⃣ Recalculer les totaux à partir des lignes
                if (invoice.Lines != null && invoice.Lines.Any())
                {
                    invoice.TotalHT = invoice.Lines.Sum(l => l.Quantity * l.UnitPriceHT);
                    invoice.TotalVAT = invoice.Lines.Sum(l => l.Quantity * l.UnitPriceHT * (l.VATRate / 100));
                    invoice.TotalTTC = invoice.TotalHT + invoice.TotalVAT;
                }
                else
                {
                    invoice.TotalHT = 0;
                    invoice.TotalVAT = 0;
                    invoice.TotalTTC = 0;
                }

                // 5️⃣ Vérifier si les montants ont changé
                if (oldTotalHT != invoice.TotalHT ||
                    oldTotalVAT != invoice.TotalVAT ||
                    oldTotalTTC != invoice.TotalTTC)
                {
                    invoice.ModifiedDate = DateTime.UtcNow;
                    invoice.ModifiedBy = "System";

                    // Optionnel: Ajouter une note
                    if (string.IsNullOrEmpty(invoice.Notes))
                        invoice.Notes = "Facture recalculée";
                    else
                        invoice.Notes += " | Facture recalculée";
                }

                // 6️⃣ Sauvegarder
                await _invoiceRepository.UpdateAsync(invoice);
                await _invoiceRepository.SaveChangesAsync(cancellationToken);

                // 7️⃣ Retourner le DTO
                var invoiceDto = _mapper.Map<InvoiceDTO>(invoice);

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
    }
}