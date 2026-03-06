using Application.Setting;
using MediatR;
using System;

namespace Application.Features.InvoiceFeature.Commands
{
    public record RecalculateInvoiceCommand(Guid Id) : IRequest<ResponseHttp>;
}