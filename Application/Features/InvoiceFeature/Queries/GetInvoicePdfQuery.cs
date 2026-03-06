using Application.Setting;
using MediatR;
using System;

namespace Application.Features.InvoiceFeature.Queries
{
    public record GetInvoicePdfQuery(Guid Id) : IRequest<ResponseHttp>;
}