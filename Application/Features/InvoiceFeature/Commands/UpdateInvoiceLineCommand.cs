using Application.Setting;
using MediatR;
using System;

namespace Application.Features.InvoiceFeature.Commands
{
    public record UpdateInvoiceLineCommand(
        Guid InvoiceId,
        Guid LineId,
        string? Description,
        int Quantity,
        string? Unit,
        decimal UnitPriceHT,
        decimal DiscountPercent,
        decimal VatRate
    ) : IRequest<ResponseHttp>;
}
