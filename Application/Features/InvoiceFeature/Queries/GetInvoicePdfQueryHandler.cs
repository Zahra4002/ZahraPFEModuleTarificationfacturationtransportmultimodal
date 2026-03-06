using Application.Features.InvoiceFeature.Dtos;
using Application.Interfaces;
using Application.Setting;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.InvoiceFeature.Queries
{
    public class GetInvoicePdfQueryHandler : IRequestHandler<GetInvoicePdfQuery, ResponseHttp>
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IPdfService _pdfService;

        public GetInvoicePdfQueryHandler(
            IInvoiceRepository invoiceRepository,
            IPdfService pdfService)
        {
            _invoiceRepository = invoiceRepository;
            _pdfService = pdfService;
        }

        public async Task<ResponseHttp> Handle(GetInvoicePdfQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // 1️⃣ Récupérer la facture avec ses détails
                var invoice = await _invoiceRepository.GetByIdWithDetailsAsync(request.Id);

                if (invoice == null)
                {
                    return new ResponseHttp
                    {
                        Status = StatusCodes.Status404NotFound,
                        Fail_Messages = $"Facture avec ID {request.Id} non trouvée"
                    };
                }

                // 2️⃣ Générer le PDF
                var pdfBytes = await _pdfService.GenerateInvoicePdfAsync(invoice);

                // 3️⃣ Créer le DTO de réponse
                var pdfResponse = new PdfResponseDto
                {
                    FileName = $"Facture_{invoice.InvoiceNumber}_{DateTime.Now:yyyyMMdd}.pdf",
                    ContentType = "application/pdf",
                    FileContent = Convert.ToBase64String(pdfBytes)
                };

                // 4️⃣ Retourner la réponse
                return new ResponseHttp
                {
                    Resultat = pdfResponse,
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