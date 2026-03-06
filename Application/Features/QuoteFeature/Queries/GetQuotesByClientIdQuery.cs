using Application.Setting;
using MediatR;
using System;

namespace Application.Features.QuoteFeature.Queries
{
    public record GetQuotesByClientIdQuery(Guid ClientId) : IRequest<ResponseHttp>;
}