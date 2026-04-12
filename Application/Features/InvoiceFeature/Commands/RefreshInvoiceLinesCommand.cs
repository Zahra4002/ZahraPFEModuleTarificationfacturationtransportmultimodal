// Application/Features/InvoiceFeature/Commands/RefreshInvoiceLinesCommand.cs
using Application.Interfaces;
using Application.Setting;
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
    public record RefreshInvoiceLinesCommand(Guid InvoiceId) : IRequest<ResponseHttp>;

    public class RefreshInvoiceLinesCommandHandler : IRequestHandler<RefreshInvoiceLinesCommand, ResponseHttp>
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IInvoiceLineRepository _invoiceLineRepository;
        private readonly IShipmentRepository _shipmentRepository;

        public RefreshInvoiceLinesCommandHandler(
            IInvoiceRepository invoiceRepository,
            IInvoiceLineRepository invoiceLineRepository,
            IShipmentRepository shipmentRepository)
        {
            _invoiceRepository = invoiceRepository;
            _invoiceLineRepository = invoiceLineRepository;
            _shipmentRepository = shipmentRepository;
        }

        public async Task<ResponseHttp> Handle(RefreshInvoiceLinesCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // 1️⃣ Récupérer la facture avec ses détails
                var invoice = await _invoiceRepository.GetByIdWithDetailsAsync(request.InvoiceId, cancellationToken);
                if (invoice == null)
                {
                    return new ResponseHttp
                    {
                        Status = StatusCodes.Status404NotFound,
                        Fail_Messages = $"Facture avec ID {request.InvoiceId} non trouvée"
                    };
                }

                // 2️⃣ Vérifier que la facture est en brouillon
                if (invoice.Status != InvoiceStatus.Brouillon)
                {
                    return new ResponseHttp
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Fail_Messages = $"Impossible de modifier les lignes d'une facture avec le statut {invoice.Status}"
                    };
                }

                // 3️⃣ Vérifier qu'une expédition est associée
                if (!invoice.ShipmentId.HasValue)
                {
                    return new ResponseHttp
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Fail_Messages = "Aucune expédition associée à cette facture"
                    };
                }

                // 4️⃣ Récupérer l'expédition avec ses segments et zones
                var shipment = await _shipmentRepository.GetShipmentWithIncludesAsync(
                    invoice.ShipmentId.Value,
                    new[] { "Segments", "Segments.ZoneFrom", "Segments.ZoneTo" },
                    cancellationToken);

                if (shipment == null)
                {
                    return new ResponseHttp
                    {
                        Status = StatusCodes.Status404NotFound,
                        Fail_Messages = "Expédition associée non trouvée"
                    };
                }

                if (shipment.Segments == null || !shipment.Segments.Any())
                {
                    return new ResponseHttp
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Fail_Messages = "L'expédition n'a pas de segments"
                    };
                }

                // 5️⃣ Supprimer les anciennes lignes
                var existingLines = await _invoiceLineRepository.GetByInvoiceIdAsync(invoice.Id);
                foreach (var line in existingLines)
                {
                    await _invoiceLineRepository.DeleteAsync(line);
                }
                await _invoiceLineRepository.SaveChangesAsync(cancellationToken);

                // 6️⃣ Créer les nouvelles lignes à partir des segments
                int lineNumber = 1;
                foreach (var segment in shipment.Segments.OrderBy(s => s.Sequence))
                {
                    // Récupérer les noms des zones
                    string originName = segment.ZoneFrom?.Name ?? segment.ZoneFromId?.ToString() ?? "Départ";
                    string destinationName = segment.ZoneTo?.Name ?? segment.ZoneToId?.ToString() ?? "Arrivée";

                    // Déterminer le mode de transport en texte
                    string transportModeText = GetTransportModeText(segment.TransportMode);

                    // Déterminer le taux de TVA
                    decimal vatRate = GetVatRateForDestination(segment.ZoneTo, shipment.DestinationAddress?.Country);

                    // Calculer les montants
                    decimal totalHT = segment.TotalCost;
                    decimal vatAmount = totalHT * (vatRate / 100);
                    decimal totalTTC = totalHT + vatAmount;

                    var line = new InvoiceLine
                    {
                        Id = Guid.NewGuid(),
                        InvoiceId = invoice.Id,
                        Description = $"Ligne {lineNumber++}: {transportModeText} - {originName} → {destinationName}",
                        Quantity = 1,
                        UnitPriceHT = totalHT,
                        VATRate = vatRate,
                        DiscountPercent = 0,
                        TransportSegmentId = segment.Id,
                        CreatedDate = DateTime.UtcNow,
                        CreatedBy = "System"
                    };

                    await _invoiceLineRepository.AddAsync(line);
                }

                await _invoiceLineRepository.SaveChangesAsync(cancellationToken);

                // 7️⃣ Recalculer les totaux de la facture
                var updatedInvoice = await _invoiceRepository.GetByIdWithLinesAsync(invoice.Id, cancellationToken);
                if (updatedInvoice != null && updatedInvoice.Lines != null && updatedInvoice.Lines.Any())
                {
                    updatedInvoice.TotalHT = updatedInvoice.Lines.Sum(l => l.Quantity * l.UnitPriceHT);
                    updatedInvoice.TotalVAT = updatedInvoice.Lines.Sum(l => l.Quantity * l.UnitPriceHT * (l.VATRate / 100));
                    updatedInvoice.TotalTTC = updatedInvoice.TotalHT + updatedInvoice.TotalVAT;
                    updatedInvoice.ModifiedDate = DateTime.UtcNow;
                    updatedInvoice.ModifiedBy = "System";

                    await _invoiceRepository.UpdateAsync(updatedInvoice);
                    await _invoiceRepository.SaveChangesAsync(cancellationToken);
                }

                // 8️⃣ Retourner la facture mise à jour avec tous les détails
                var finalInvoice = await _invoiceRepository.GetByIdWithDetailsAsync(invoice.Id, cancellationToken);

                return new ResponseHttp
                {
                    Status = StatusCodes.Status200OK,
                    Resultat = finalInvoice,
                    Fail_Messages = null
                };
            }
            catch (Exception ex)
            {
                return new ResponseHttp
                {
                    Status = StatusCodes.Status400BadRequest,
                    Fail_Messages = $"Erreur lors de la régénération des lignes: {ex.Message}"
                };
            }
        }

        private string GetTransportModeText(TransportMode mode)
        {
            return mode switch
            {
                TransportMode.Maritime => "Maritime",
                TransportMode.Aerien => "Aérien",
                TransportMode.Routier => "Routier",
                TransportMode.Ferroviaire => "Ferroviaire",
                TransportMode.Fluvial => "Fluvial",
                _ => "Transport"
            };
        }

        private decimal GetVatRateForDestination(Zone? zoneTo, string? destinationCountry)
        {
            // Priorité 1: Taux de TVA défini sur la zone
            if (zoneTo != null && zoneTo.TaxRate.HasValue && zoneTo.TaxRate.Value > 0)
            {
                return zoneTo.TaxRate.Value;
            }

            // Priorité 2: Taux par pays
            string country = zoneTo?.Country ?? destinationCountry ?? "FR";

            var countryLower = country.ToLower();

            return countryLower switch
            {
                "france" or "fr" => 20,
                "tunisie" or "tn" => 19,
                "maroc" or "ma" => 20,
                "algérie" or "dz" => 19,
                "sénégal" or "sn" => 18,
                "côte d'ivoire" or "ci" => 18,
                "cameroun" or "cm" => 19.25m,
                "burkina faso" or "bf" => 18,
                "mali" or "ml" => 18,
                "niger" or "ne" => 19,
                "tchad" or "td" => 18,
                "togo" or "tg" => 18,
                "bénin" or "bj" => 18,
                "guinée" or "gn" => 18,
                "mauritanie" or "mr" => 16,
                "libye" or "ly" => 17,
                "égypte" or "eg" => 14,
                "arabie saoudite" or "sa" => 15,
                "émirats arabes unis" or "ae" => 5,
                "qatar" or "qa" => 0,
                "koweït" or "kw" => 5,
                "oman" or "om" => 5,
                "bahreïn" or "bh" => 10,
                "jordanie" or "jo" => 16,
                "liban" or "lb" => 11,
                "italie" or "it" => 22,
                "allemagne" or "de" => 19,
                "espagne" or "es" => 21,
                "belgique" or "be" => 21,
                "pays-bas" or "nl" => 21,
                "portugal" or "pt" => 23,
                "suisse" or "ch" => 7.7m,
                "royaume-uni" or "gb" or "uk" => 20,
                "états-unis" or "us" => 0,
                _ => 20
            };
        }
    }
}