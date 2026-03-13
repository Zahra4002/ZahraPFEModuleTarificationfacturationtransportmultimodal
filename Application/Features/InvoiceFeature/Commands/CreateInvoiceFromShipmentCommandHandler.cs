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
    public class CreateInvoiceFromShipmentCommandHandler : IRequestHandler<CreateInvoiceFromShipmentCommand, ResponseHttp>
    {
        private readonly IShipmentRepository _shipmentRepository;
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IZoneRepository _zoneRepository;  // ← AJOUTÉ pour les zones
        private readonly IMapper _mapper;

        public CreateInvoiceFromShipmentCommandHandler(
            IShipmentRepository shipmentRepository,
            IInvoiceRepository invoiceRepository,
            IZoneRepository zoneRepository,  // ← AJOUTÉ
            IMapper mapper)
        {
            _shipmentRepository = shipmentRepository;
            _invoiceRepository = invoiceRepository;
            _zoneRepository = zoneRepository;
            _mapper = mapper;
        }

        public async Task<ResponseHttp> Handle(CreateInvoiceFromShipmentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // 1️⃣ Vérifier que ShipmentId n'est pas null
               
                {
                    if (request.ShipmentId == Guid.Empty)
                        return new ResponseHttp
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Fail_Messages = "ShipmentId est requis"
                    };
                }

                // 2️⃣ Récupérer l'expédition avec ses segments et les zones
                var shipment = await _shipmentRepository.GetByIdWithDetailsAsync(request.ShipmentId);

                if (shipment == null)
                {
                    return new ResponseHttp
                    {
                        Status = StatusCodes.Status404NotFound,
                        Fail_Messages = $"Expédition avec ID {request.ShipmentId} non trouvée"
                    };
                }

                // 3️⃣ Vérifier si une facture existe déjà
                var existingInvoice = await _invoiceRepository.GetByShipmentIdAsync(request.ShipmentId);
                if (existingInvoice != null)
                {
                    return new ResponseHttp
                    {
                        Status = StatusCodes.Status409Conflict,
                        Fail_Messages = $"Une facture existe déjà pour cette expédition (N° {existingInvoice.InvoiceNumber})"
                    };
                }

                // 4️⃣ Calculer les montants
                decimal totalHT = 0;

                if (shipment.Segments != null)
                {
                    foreach (var segment in shipment.Segments)
                    {
                        totalHT += segment.TotalCost;
                    }
                }

                decimal totalVAT = totalHT * 0.19m; // TVA 19% par défaut
                decimal totalTTC = totalHT + totalVAT;

                // 5️⃣ Générer un numéro de facture
                var invoiceNumber = await GenerateInvoiceNumberAsync();

                // 6️⃣ Déterminer la devise (depuis Shipment)
                string currencyCode = !string.IsNullOrEmpty(shipment.CurrencyCode)
                    ? shipment.CurrencyCode
                    : "EUR";

                // 7️⃣ Créer la facture
                var invoice = new Invoice
                {
                    Id = Guid.NewGuid(),
                    InvoiceNumber = invoiceNumber,
                    ClientId = shipment.ClientId,
                    ShipmentId = shipment.Id,
                    ShipmentNumber = shipment.ShipmentNumber,
                    InvoiceDate = DateTime.UtcNow,
                    DueDate = DateTime.UtcNow.AddDays(30),
                    Status = InvoiceStatus.Brouillon,
                    TotalHT = totalHT,
                    TotalVAT = totalVAT,
                    TotalTTC = totalTTC,
                    AmountPaid = 0,
                    CurrencyId = null, // À mapper si tu as une table Currency
                    ExchangeRate = 1,
                    Notes = $"Facture générée depuis l'expédition {shipment.ShipmentNumber}",
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = "System",
                    Lines = new System.Collections.Generic.List<InvoiceLine>()
                };

                // 8️⃣ Créer les lignes de facture avec les informations des zones
                if (shipment.Segments != null)
                {
                    foreach (var segment in shipment.Segments)
                    {
                        // Récupérer les noms des zones
                        string originName = "Inconnu";
                        string destinationName = "Inconnu";

                        if (segment.ZoneFromet != null)
                            originName = segment.ZoneFromet.Name ?? "Zone " + segment.ZoneFromId;

                        if (segment.ZoneTo != null)
                            destinationName = segment.ZoneTo.Name ?? "Zone " + segment.ZoneToId;

                        var line = new InvoiceLine
                        {
                            Id = Guid.NewGuid(),
                            InvoiceId = invoice.Id,
                            Description = $"Transport {originName} → {destinationName} ({segment.TransportMode})",
                            Quantity = 1,
                            UnitPriceHT = segment.TotalCost,
                            VATRate = GetVATRate(segment.TransportMode),
                            TransportSegmentId = segment.Id
                        };

                        invoice.Lines.Add(line);
                    }
                }

                // 9️⃣ Sauvegarder
                await _invoiceRepository.AddAsync(invoice);
                await _invoiceRepository.SaveChangesAsync(cancellationToken);

                // 🔟 Retourner le DTO
                var invoiceDto = _mapper.Map<InvoiceDTO>(invoice);

                return new ResponseHttp
                {
                    Resultat = invoiceDto,
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

        private async Task<string> GenerateInvoiceNumberAsync()
        {
            var lastInvoice = await _invoiceRepository.GetLastInvoiceAsync();
            var lastNumber = 0;

            if (lastInvoice != null && !string.IsNullOrEmpty(lastInvoice.InvoiceNumber))
            {
                var parts = lastInvoice.InvoiceNumber.Split('-');
                if (parts.Length > 1 && int.TryParse(parts.Last(), out int num))
                {
                    lastNumber = num;
                }
            }

            var newNumber = (lastNumber + 1).ToString("D4");
            return $"INV-{DateTime.UtcNow:yyyyMMdd}-{newNumber}";
        }

        private decimal GetVATRate(TransportMode mode)
        {
            // Logique TVA: 0% pour maritime, 19% pour les autres
            return mode == TransportMode.Maritime ? 0 : 0.19m;
        }
    }
}