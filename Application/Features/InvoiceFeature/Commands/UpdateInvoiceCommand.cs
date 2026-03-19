using Application.Features.InvoiceFeature.Dtos;
using Application.Interfaces;
using Application.Setting;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Features.InvoiceFeature.Commands
{
    public record UpdateInvoiceCommand(
        Guid InvoiceId,
        Guid SupplierId,
        string InvoiceNumber,
        Guid ClientId,
        Guid? ShipmentId,
        DateTime IssueDate,
        DateTime DueDate,
        Guid? CurrencyId,
        decimal ExchangeRate,
        string? Notes
    ) : IRequest<ResponseHttp>;

    public class UpdateInvoiceCommandHandler
        : IRequestHandler<UpdateInvoiceCommand, ResponseHttp>
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IMapper _mapper;

        public UpdateInvoiceCommandHandler(
            IInvoiceRepository invoiceRepository,
            IMapper mapper)
        {
            _invoiceRepository = invoiceRepository;
            _mapper = mapper;
        }

        public async Task<ResponseHttp> Handle(
            UpdateInvoiceCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                // ✅ Récupérer l'invoice avec PascalCase
                Invoice? invoice = await _invoiceRepository.GetById(request.InvoiceId);

                if (invoice == null)
                {
                    return new ResponseHttp
                    {
                        Resultat = null,  // ✅ Pas de mapping quand null
                        Fail_Messages = "Invoice with this Id not found.",
                        Status = StatusCodes.Status404NotFound,  // ✅ 404 au lieu de 400
                    };
                }

                // ✅ Mise à jour des propriétés avec PascalCase
                invoice.SupplierId = request.SupplierId;
                invoice.InvoiceNumber = request.InvoiceNumber;
                invoice.ClientId = request.ClientId;
                invoice.ShipmentId = request.ShipmentId;
                invoice.IssueDate = DateTime.SpecifyKind(request.IssueDate, DateTimeKind.Utc);
                invoice.DueDate = DateTime.SpecifyKind(request.DueDate, DateTimeKind.Utc);
                invoice.CurrencyId = request.CurrencyId;
                invoice.ExchangeRate = request.ExchangeRate;
                invoice.Notes = request.Notes;

                // ✅ Ajouter les métadonnées
                invoice.ModifiedDate = DateTime.UtcNow;
                invoice.ModifiedBy = "System";

                // ✅ Sauvegarder
                await _invoiceRepository.Update(invoice);
                await _invoiceRepository.SaveChange(cancellationToken);

                // ✅ Recharger depuis la base pour récupérer les relations
                var updatedInvoice = await _invoiceRepository.GetByIdWithDetailsAsync(invoice.Id, cancellationToken);

                // ✅ Mapper et retourner
                var invoiceToReturn = _mapper.Map<InvoiceDTO>(updatedInvoice);

                return new ResponseHttp
                {
                    Resultat = invoiceToReturn,
                    Status = StatusCodes.Status200OK,
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