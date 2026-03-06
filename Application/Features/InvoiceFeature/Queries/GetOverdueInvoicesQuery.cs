using Application.Setting;
using MediatR;
using Application.Features.InvoiceFeature.Dtos;

namespace Application.Features.InvoiceFeature.Queries
{
    public record GetOverdueInvoicesQuery() : IRequest<ResponseHttp>;
}