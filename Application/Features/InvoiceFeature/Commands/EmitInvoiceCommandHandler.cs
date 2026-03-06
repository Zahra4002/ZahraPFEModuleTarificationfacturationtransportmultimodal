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
    public class EmitInvoiceCommandHandler : IRequestHandler<EmitInvoiceCommand, ResponseHttp>
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IMapper _mapper;

        public EmitInvoiceCommandHandler(
            IInvoiceRepository invoiceRepository,
            IMapper mapper)
        {
            _invoiceRepository = invoiceRepository;
            _mapper = mapper;
        }

        public async Task<ResponseHttp> Handle(EmitInvoiceCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // 1️⃣ Récupérer la facture
                var invoice = await _invoiceRepository.GetByIdAsync(request.Id);
                if (invoice == null)
                    return new ResponseHttp
                    {
                        Status = StatusCodes.Status404NotFound,
                        Fail_Messages = $"Facture avec ID {request.Id} non trouvée"
                    };

                // 2️⃣ Vérifier le statut
                if (invoice.Status != InvoiceStatus.Brouillon)
                    return new ResponseHttp
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Fail_Messages = $"Impossible d'émettre une facture avec le statut {invoice.Status}"
                    };

                // 3️⃣ Vérifier les lignes
                if (invoice.Lines == null || invoice.Lines.Count == 0)
                    return new ResponseHttp
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Fail_Messages = "Impossible d'émettre une facture sans lignes"
                    };

                // 4️⃣ Vérifier le montant
                if (invoice.TotalHT <= 0)
                    return new ResponseHttp
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Fail_Messages = "Impossible d'émettre une facture avec un montant HT <= 0"
                    };

                // 5️⃣ Mettre à jour la facture
                invoice.Status = InvoiceStatus.Emise;
                invoice.ModifiedDate = DateTime.UtcNow;
                invoice.ModifiedBy = "System";

                // ✅ Gestion sécurisée des DateTime pour PostgreSQL
                if (request.InvoiceDate != default)
                    invoice.InvoiceDate = DateTime.SpecifyKind(request.InvoiceDate, DateTimeKind.Utc);

                if (request.DueDate != default)
                    invoice.DueDate = DateTime.SpecifyKind(request.DueDate, DateTimeKind.Utc);

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