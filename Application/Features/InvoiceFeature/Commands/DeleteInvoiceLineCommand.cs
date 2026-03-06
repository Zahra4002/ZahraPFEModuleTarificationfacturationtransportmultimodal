using Application.Setting;
using MediatR;
using System;

namespace Application.Features.InvoiceFeature.Commands
{
    public record DeleteInvoiceLineCommand(
        Guid InvoiceId,
        Guid LineId
    ) : IRequest<ResponseHttp>;
}