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
        Guid invoiceId,
        string invoiceNumber,
        Guid clientId,
        Guid? shipmentId,
        DateTime issueDate,
        DateTime dueDate,
        Guid? currencyId,
        decimal exchangeRate,
        string? notes
    ) : IRequest<ResponseHttp>
    {
        internal readonly object Id;

        public class UpdateInvoiceCommandHandler
            : IRequestHandler<UpdateInvoiceCommand, ResponseHttp>
        {
            private readonly IInvoiceRepository invoiceRepository;
            private readonly IMapper _mapper;

            public UpdateInvoiceCommandHandler(
                IInvoiceRepository invoiceRepository,
                IMapper mapper)
            {
                this.invoiceRepository = invoiceRepository;
                _mapper = mapper;
            }

            public async Task<ResponseHttp> Handle(
                UpdateInvoiceCommand request,
                CancellationToken cancellationToken)
            {
                Invoice? invoice = await invoiceRepository.GetById(request.invoiceId);

                if (invoice == null)
                {
                    return new ResponseHttp
                    {
                        Resultat = _mapper.Map<InvoiceDTO>(invoice),
                        Fail_Messages = "Invoice with this Id not found.",
                        Status = StatusCodes.Status400BadRequest,
                    };
                }
                else
                {
                    // Mise à jour des propriétés
                    invoice.InvoiceNumber = request.invoiceNumber;
                    invoice.ClientId = request.clientId;
                    invoice.ShipmentId = request.shipmentId;
                    invoice.IssueDate = request.issueDate;
                    invoice.DueDate = request.dueDate;
                    invoice.CurrencyId = request.currencyId;
                    invoice.ExchangeRate = request.exchangeRate;
                    invoice.Notes = request.notes;

                    await invoiceRepository.Update(invoice);
                    await invoiceRepository.SaveChange(cancellationToken);

                    var invoiceToReturn = _mapper.Map<InvoiceDTO>(invoice);

                    return new ResponseHttp
                    {
                        Resultat = invoiceToReturn,
                        Status = StatusCodes.Status200OK,
                    };
                }
            }
        }
    }
}