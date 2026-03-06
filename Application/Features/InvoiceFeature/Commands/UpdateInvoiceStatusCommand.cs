using Application.Setting;
using Domain.Enums;
using MediatR;
using System;

namespace Application.Features.InvoiceFeature.Commands
{
    public record UpdateInvoiceStatusCommand(Guid Id, InvoiceStatus Status) : IRequest<ResponseHttp>;
}