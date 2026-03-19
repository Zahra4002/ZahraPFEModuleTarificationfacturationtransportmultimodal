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
    public record AddInvoiceCommand(
        string InvoiceNumber,
        Guid? supplierId,
        Guid? ClientId,
        Guid? ShipmentId,
        DateTime IssueDate,
        DateTime DueDate,
        Guid? CurrencyId,
        decimal? ExchangeRate,
        string? Notes        
    ) : IRequest<ResponseHttp>;

    public class AddInvoiceCommandHandler
        : IRequestHandler<AddInvoiceCommand, ResponseHttp>
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IMapper _mapper;

        public AddInvoiceCommandHandler(
            IInvoiceRepository invoiceRepository,
            IMapper mapper)
        {
            _invoiceRepository = invoiceRepository;
            _mapper = mapper;
        }

        public async Task<ResponseHttp> Handle(
            AddInvoiceCommand request,
            CancellationToken cancellationToken)

        {
            try
            {
                // 1️⃣ Mapping Command → Entity
                var invoice = _mapper.Map<Invoice>(request);
                invoice.Id = Guid.NewGuid();

                // ✅ Convertir les dates en UTC
                invoice.InvoiceDate = DateTime.SpecifyKind(request.IssueDate, DateTimeKind.Utc);
                invoice.DueDate = DateTime.SpecifyKind(request.DueDate, DateTimeKind.Utc);

                

                // 3️⃣ Logique métier
                invoice.Status = InvoiceStatus.Brouillon;
                invoice.AmountPaid = 0;
                invoice.CreatedDate = DateTime.UtcNow;
                invoice.CreatedBy = "System";

                // 4️⃣ Sauvegarde
                await _invoiceRepository.Post(invoice);
                await _invoiceRepository.SaveChange(cancellationToken);
                

                // 🔥 Recharge depuis la base
                var savedInvoice = await _invoiceRepository
                   .GetByIdWithDetailsAsync(invoice.Id, cancellationToken);

                // 5️⃣ Retour DTO
                return new ResponseHttp
                {
                    Resultat = _mapper.Map<InvoiceDTO>(savedInvoice),
                    Status = StatusCodes.Status200OK
                };
            }
            catch (Exception ex)
            {
                return new ResponseHttp
                {
                    Fail_Messages = ex.Message,
                    Status = StatusCodes.Status400BadRequest,
                };
            }
        }
    }
}