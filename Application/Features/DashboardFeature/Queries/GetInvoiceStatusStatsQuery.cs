using Application.Setting;
using MediatR;

namespace Application.Features.DashboardFeature.Queries
{
    public record GetInvoiceStatusStatsQuery() : IRequest<ResponseHttp>;
}