using Application.Setting;
using MediatR;
using System;

namespace Application.Features.InvoiceFeature.Commands
{
    public record AddInvoiceLineCommand(
        Guid InvoiceId,
        string? Description,
        int Quantity,
        string? Unit,
        decimal UnitPriceHT,
        decimal DiscountPercent,
        decimal VatRate,
        Guid? TransportSegmentId,
        Guid? SurchargeId
    ) : IRequest<ResponseHttp>;
}
