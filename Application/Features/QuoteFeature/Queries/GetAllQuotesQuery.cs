using Application.Setting;
using Domain.Common;
using MediatR;

namespace Application.Features.QuoteFeature.Queries
{
    public record GetAllQuotesQuery(int? PageNumber, int? PageSize) : IRequest<ResponseHttp>;
}