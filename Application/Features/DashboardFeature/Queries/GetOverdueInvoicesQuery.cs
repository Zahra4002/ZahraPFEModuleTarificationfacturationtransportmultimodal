using Application.Setting;
using MediatR;

namespace Application.Features.DashboardFeature.Queries
{
    public record GetOverdueInvoicesQuery() : IRequest<ResponseHttp>;
}
