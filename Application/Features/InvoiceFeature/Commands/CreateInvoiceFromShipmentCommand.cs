using Application.Setting;
using MediatR;
using System;

namespace Application.Features.InvoiceFeature.Commands
{
    public record CreateInvoiceFromShipmentCommand(
        Guid ShipmentId,
        Guid? CurrencyId
    ) : IRequest<ResponseHttp>;
}