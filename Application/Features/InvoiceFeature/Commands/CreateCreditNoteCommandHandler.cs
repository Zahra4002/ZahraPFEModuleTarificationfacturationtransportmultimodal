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
    public class CreateCreditNoteCommandHandler : IRequestHandler<CreateCreditNoteCommand, ResponseHttp>
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IMapper _mapper;

        public CreateCreditNoteCommandHandler(
            IInvoiceRepository invoiceRepository,
            IMapper mapper)
        {
            _invoiceRepository = invoiceRepository;
            _mapper = mapper;
        }

        public async Task<ResponseHttp> Handle(CreateCreditNoteCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // 1️⃣ Récupérer la facture originale
                var originalInvoice = await _invoiceRepository.GetByIdWithDetailsAsync(request.Id);

                if (originalInvoice == null)
                {
                    return new ResponseHttp
                    {
                        Status = StatusCodes.Status404NotFound,
                        Fail_Messages = $"Facture avec ID {request.Id} non trouvée"
                    };
                }

                // 2️⃣ Vérifier que la facture peut avoir un avoir
                if (originalInvoice.Status == InvoiceStatus.Brouillon)
                {
                    return new ResponseHttp
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Fail_Messages = "Impossible de créer un avoir pour une facture en brouillon"
                    };
                }

                // 3️⃣ Générer un numéro d'avoir unique
                var creditNoteNumber = await GenerateCreditNoteNumberAsync();

                // 4️⃣ Créer l'avoir (comme une facture négative)
                var creditNote = new Invoice
                {
                    Id = Guid.NewGuid(),
                    InvoiceNumber = creditNoteNumber,
                    ClientId = originalInvoice.ClientId,
                    Client = originalInvoice.Client,
                    ShipmentId = originalInvoice.ShipmentId,
                    ShipmentNumber = originalInvoice.ShipmentNumber,
                    InvoiceDate = DateTime.UtcNow,
                    DueDate = DateTime.UtcNow.AddDays(30), // Échéance standard
                    Status = InvoiceStatus.Emise, // L'avoir est émis directement
                    TotalHT = -originalInvoice.TotalHT, // Montants négatifs
                    TotalVAT = -originalInvoice.TotalVAT,
                    TotalTTC = -originalInvoice.TotalTTC,
                    AmountPaid = 0,
                    CurrencyId = originalInvoice.CurrencyId,
                    
                    ExchangeRate = originalInvoice.ExchangeRate,
                    Notes = $"Avoir pour la facture {originalInvoice.InvoiceNumber}",
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = "System",
                    Lines = new System.Collections.Generic.List<InvoiceLine>()
                };

                // 5️⃣ Copier les lignes avec des montants négatifs
                foreach (var line in originalInvoice.Lines)
                {
                    var creditLine = new InvoiceLine
                    {
                        Id = Guid.NewGuid(),
                        InvoiceId = creditNote.Id,
                        Description = $"Avoir: {line.Description}",
                        Quantity = line.Quantity,
                        UnitPriceHT = -line.UnitPriceHT, // Négatif
                        VATRate = line.VATRate,
                        DiscountPercent = line.DiscountPercent,
                        TransportSegmentId = line.TransportSegmentId,
                        CreatedDate = DateTime.UtcNow
                    };
                    creditNote.Lines.Add(creditLine);
                }

                // 6️⃣ Sauvegarder l'avoir
                await _invoiceRepository.AddAsync(creditNote);
                await _invoiceRepository.SaveChangesAsync(cancellationToken);

                // 7️⃣ Retourner le DTO (InvoiceDTO comme dans l'exemple)
                var creditNoteDto = _mapper.Map<InvoiceDTO>(creditNote);

                return new ResponseHttp
                {
                    Resultat = creditNoteDto,
                    Status = StatusCodes.Status201Created
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

        private async Task<string> GenerateCreditNoteNumberAsync()
        {
            var lastInvoice = await _invoiceRepository.GetLastInvoiceAsync();
            var lastNumber = 0;

            if (lastInvoice != null && !string.IsNullOrEmpty(lastInvoice.InvoiceNumber))
            {
                // Chercher le dernier avoir (commençant par CN-)
                var parts = lastInvoice.InvoiceNumber.Split('-');
                if (parts.Length > 1 && parts[0] == "CN" && int.TryParse(parts.Last(), out int num))
                {
                    lastNumber = num;
                }
            }

            var newNumber = (lastNumber + 1).ToString("D4");
            return $"CN-{DateTime.UtcNow:yyyyMMdd}-{newNumber}";
        }
    }
}