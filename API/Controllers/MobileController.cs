// API/Controllers/MobileController.cs
using Application.Interfaces;
using Application.Setting;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace API.Controllers
{
    [Route("api/mobile")]
    [ApiController]
    [Authorize]
    public class MobileController : ControllerBase
    {
        private readonly ICleanArchitecturContext _context;
        private readonly IUserRepository _userRepository;
        private readonly IClientRepository _clientRepository;
        private readonly IShipmentRepository _shipmentRepository;
        private readonly IQuoteRepository _quoteRepository;
        private readonly IInvoiceRepository _invoiceRepository;

        public MobileController(
            ICleanArchitecturContext context,
            IUserRepository userRepository,
            IClientRepository clientRepository,
            IShipmentRepository shipmentRepository,
            IQuoteRepository quoteRepository,
            IInvoiceRepository invoiceRepository)
        {
            _context = context;
            _userRepository = userRepository;
            _clientRepository = clientRepository;
            _shipmentRepository = shipmentRepository;
            _quoteRepository = quoteRepository;
            _invoiceRepository = invoiceRepository;
        }

        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                              ?? User.FindFirst("sub")?.Value;
            return Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
        }

        private async Task<Client?> GetCurrentClientAsync(CancellationToken cancellationToken = default)
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty) return null;

            var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
            if (user == null) return null;

            // Remplacer SelectOneAsync par FirstOrDefault
            return await _context.Clients
                .FirstOrDefaultAsync(c => c.Email == user.Email && !c.IsDeleted, cancellationToken);
        }

        // ==================== PROFIL CLIENT ====================

        [HttpGet("client/profile")]
        public async Task<IActionResult> GetClientProfile(CancellationToken cancellationToken)
        {
            var client = await GetCurrentClientAsync(cancellationToken);
            if (client == null)
                return NotFound(new { message = "Client non trouvé" });

            return Ok(new ResponseHttp
            {
                Resultat = new
                {
                    client.Id,
                    client.Code,
                    client.Name,
                    client.Email,
                    client.PhoneNumber,
                    client.DefaultCurrencyCode,
                    client.IsActive
                },
                Status = 200
            });
        }

        // ==================== EXPÉDITIONS ====================

        [HttpGet("client/shipments")]
        public async Task<IActionResult> GetClientShipments(CancellationToken cancellationToken)
        {
            var client = await GetCurrentClientAsync(cancellationToken);
            if (client == null)
                return NotFound(new { message = "Client non trouvé" });

            var shipments = await _context.Shipments
                .Where(s => s.ClientId == client.Id && !s.IsDeleted)
                .OrderByDescending(s => s.CreatedDate)
                .Select(s => new
                {
                    s.Id,
                    s.ShipmentNumber,
                    s.Status,
                    OriginAddress = s.OriginAddress.City + ", " + s.OriginAddress.Country,
                    DestinationAddress = s.DestinationAddress.City + ", " + s.DestinationAddress.Country,
                    s.CreatedDate,
                    Progress = s.Segments.Count > 0
                        ? (double)s.Segments.Count(seg => seg.Status == SegmentStatus.Completed) / s.Segments.Count * 100
                        : 0
                })
                .ToListAsync(cancellationToken);

            return Ok(new ResponseHttp
            {
                Resultat = new { items = shipments, totalCount = shipments.Count },
                Status = 200
            });
        }

        [HttpGet("shipments/{id}/tracking")]
        public async Task<IActionResult> GetShipmentTracking(Guid id, CancellationToken cancellationToken)
        {
            var client = await GetCurrentClientAsync(cancellationToken);
            if (client == null)
                return NotFound(new { message = "Client non trouvé" });

            var shipment = await _context.Shipments
                .Include(s => s.Segments)
                    .ThenInclude(seg => seg.ZoneFrom)
                .Include(s => s.Segments)
                    .ThenInclude(seg => seg.ZoneTo)
                .FirstOrDefaultAsync(s => s.Id == id && s.ClientId == client.Id && !s.IsDeleted, cancellationToken);

            if (shipment == null)
                return NotFound(new { message = "Expédition non trouvée" });

            var sortedSegments = shipment.Segments.OrderBy(s => s.Sequence).ToList();
            var completedCount = sortedSegments.Count(s => s.Status == SegmentStatus.Completed);
            var progress = sortedSegments.Count > 0 ? (double)completedCount / sortedSegments.Count * 100 : 0;

            var tracking = new
            {
                shipment.Id,
                shipment.ShipmentNumber,
                shipment.Status,
                Progress = progress,
                CurrentLocation = GetCurrentLocation(sortedSegments),
                EstimatedDelivery = GetEstimatedDelivery(sortedSegments),
                Segments = sortedSegments.Select(seg => new
                {
                    seg.Id,
                    seg.Sequence,
                    TransportMode = seg.TransportMode.ToString(),
                    FromLocation = seg.ZoneFrom?.Name ?? "Départ",
                    ToLocation = seg.ZoneTo?.Name ?? "Arrivée",
                    seg.DepartureDate,
                    seg.ArrivalDate,
                    Status = seg.Status.ToString()
                })
            };

            return Ok(new ResponseHttp { Resultat = tracking, Status = 200 });
        }

        private string GetCurrentLocation(List<TransportSegment> segments)
        {
            var activeSegment = segments.FirstOrDefault(s => s.Status == SegmentStatus.InProgress);
            if (activeSegment != null)
                return activeSegment.ZoneFrom?.Name ?? "En transit";

            var lastCompleted = segments.LastOrDefault(s => s.Status == SegmentStatus.Completed);
            if (lastCompleted != null)
                return lastCompleted.ZoneTo?.Name ?? "En cours";

            return segments.FirstOrDefault()?.ZoneFrom?.Name ?? "Départ";
        }

        private DateTime? GetEstimatedDelivery(List<TransportSegment> segments)
        {
            var lastSegment = segments.LastOrDefault();
            return lastSegment?.ArrivalDate;
        }

        // ==================== DEVIS ====================

        [HttpGet("client/quotes")]
        public async Task<IActionResult> GetClientQuotes(CancellationToken cancellationToken)
        {
            var client = await GetCurrentClientAsync(cancellationToken);
            if (client == null)
                return NotFound(new { message = "Client non trouvé" });

            var quotes = await _context.Quotes
                .Where(q => q.ClientId == client.Id && !q.IsDeleted)
                .OrderByDescending(q => q.CreatedDate)
                .Select(q => new
                {
                    q.Id,
                    q.QuoteNumber,
                    q.TotalHT,
                    q.TotalTTC,
                    q.CurrencyCode,
                    q.ValidUntil,
                    q.IsAccepted,
                    q.CreatedDate,
                    IsExpired = q.ValidUntil < DateTime.UtcNow,
                    OriginAddress = q.OriginAddress.City + ", " + q.OriginAddress.Country,
                    DestinationAddress = q.DestinationAddress.City + ", " + q.DestinationAddress.Country
                })
                .ToListAsync(cancellationToken);

            return Ok(new ResponseHttp
            {
                Resultat = new { items = quotes, totalCount = quotes.Count },
                Status = 200
            });
        }

        [HttpPost("quotes/{id}/accept")]
        public async Task<IActionResult> AcceptQuote(Guid id, CancellationToken cancellationToken)
        {
            var client = await GetCurrentClientAsync(cancellationToken);
            if (client == null)
                return NotFound(new { message = "Client non trouvé" });

            var quote = await _context.Quotes
                .FirstOrDefaultAsync(q => q.Id == id && q.ClientId == client.Id && !q.IsDeleted, cancellationToken);

            if (quote == null)
                return NotFound(new { message = "Devis non trouvé" });

            if (quote.IsAccepted)
                return BadRequest(new { message = "Ce devis a déjà été accepté" });

            if (quote.ValidUntil < DateTime.UtcNow)
                return BadRequest(new { message = "Ce devis a expiré" });

            quote.IsAccepted = true;
            quote.AcceptedAt = DateTime.UtcNow;
            quote.ModifiedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            return Ok(new ResponseHttp
            {
                Resultat = new { quote.Id, quote.QuoteNumber, IsAccepted = true },
                Status = 200
            });
        }

        [HttpPost("quotes/{id}/reject")]
        public async Task<IActionResult> RejectQuote(Guid id, CancellationToken cancellationToken)
        {
            var client = await GetCurrentClientAsync(cancellationToken);
            if (client == null)
                return NotFound(new { message = "Client non trouvé" });

            var quote = await _context.Quotes
                .FirstOrDefaultAsync(q => q.Id == id && q.ClientId == client.Id && !q.IsDeleted, cancellationToken);

            if (quote == null)
                return NotFound(new { message = "Devis non trouvé" });

            quote.IsDeleted = true;
            quote.ModifiedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            return Ok(new ResponseHttp
            {
                Resultat = new { quote.Id, quote.QuoteNumber, Rejected = true },
                Status = 200
            });
        }

        // ==================== FACTURES ====================

        [HttpGet("client/invoices")]
        public async Task<IActionResult> GetClientInvoices(CancellationToken cancellationToken)
        {
            var client = await GetCurrentClientAsync(cancellationToken);
            if (client == null)
                return NotFound(new { message = "Client non trouvé" });

            var invoices = await _context.Invoices
                .Where(i => i.ClientId == client.Id && !i.IsDeleted)
                .OrderByDescending(i => i.InvoiceDate)
                .Select(i => new
                {
                    i.Id,
                    i.InvoiceNumber,
                    i.InvoiceDate,
                    i.DueDate,
                    i.TotalHT,
                    i.TotalTTC,
                    i.AmountPaid,
                    i.CurrencyCode,
                    Status = i.Status.ToString(),
                    Balance = i.TotalTTC - i.AmountPaid,
                    IsOverdue = i.DueDate < DateTime.UtcNow && i.Status != InvoiceStatus.Payee
                })
                .ToListAsync(cancellationToken);

            return Ok(new ResponseHttp
            {
                Resultat = new { items = invoices, totalCount = invoices.Count },
                Status = 200
            });
        }




        // API/Controllers/MobileController.cs
        // Ajouter cette méthode après GetClientInvoices

        // ==================== DÉTAIL FACTURE CLIENT ====================

        [HttpGet("client/invoices/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetClientInvoiceDetail(Guid id, CancellationToken cancellationToken)
        {
            // 1️⃣ Récupérer le client connecté
            var client = await GetCurrentClientAsync(cancellationToken);
            if (client == null)
                return Unauthorized(new { message = "Client non trouvé" });

            // 2️⃣ Récupérer la facture du client avec ses lignes
            var invoice = await _context.Invoices
                .Include(i => i.Lines)
                .FirstOrDefaultAsync(i => i.Id == id && i.ClientId == client.Id && !i.IsDeleted, cancellationToken);

            if (invoice == null)
                return NotFound(new { message = "Facture non trouvée" });

            // 3️⃣ Construire la réponse
            var result = new
            {
                invoice.Id,
                invoice.InvoiceNumber,
                invoice.InvoiceDate,
                invoice.DueDate,
                invoice.TotalHT,
                invoice.TotalTTC,
                invoice.AmountPaid,
                invoice.CurrencyCode,
                Status = invoice.Status.ToString(),
                Balance = invoice.TotalTTC - invoice.AmountPaid,
                IsOverdue = invoice.DueDate < DateTime.UtcNow && invoice.Status != InvoiceStatus.Payee,
                Lines = invoice.Lines.Select(l => new
                {
                    l.Id,
                    l.Description,
                    l.Quantity,
                    l.UnitPriceHT,
                    VATRate = l.VATRate,
                    TotalHT = l.Quantity * l.UnitPriceHT,
                    TotalTTC = l.Quantity * l.UnitPriceHT * (1 + l.VATRate / 100)
                })
            };

            return Ok(new ResponseHttp
            {
                Resultat = result,
                Status = 200
            });
        }
        [HttpGet("invoices/{id}/pdf")]
        public async Task<IActionResult> GetInvoicePdf(Guid id, CancellationToken cancellationToken)
        {
            var client = await GetCurrentClientAsync(cancellationToken);
            if (client == null)
                return NotFound(new { message = "Client non trouvé" });

            var invoice = await _context.Invoices
                .Include(i => i.Client)
                .Include(i => i.Lines)
                .FirstOrDefaultAsync(i => i.Id == id && i.ClientId == client.Id && !i.IsDeleted, cancellationToken);

            if (invoice == null)
                return NotFound(new { message = "Facture non trouvée" });

            var pdfBytes = await GenerateInvoicePdfAsync(invoice, cancellationToken);

            return File(pdfBytes, "application/pdf", $"Facture_{invoice.InvoiceNumber}.pdf");
        }

        private async Task<byte[]> GenerateInvoicePdfAsync(Invoice invoice, CancellationToken cancellationToken)
        {
            // TODO: Implémenter avec QuestPDF
            await Task.Delay(100, cancellationToken);
            return Array.Empty<byte>();
        }
    }
}