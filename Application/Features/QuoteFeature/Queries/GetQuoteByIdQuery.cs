using Application.Setting;
using MediatR;
using System;

namespace Application.Features.QuoteFeature.Queries
{
    public record GetQuoteByIdQuery(Guid Id) : IRequest<ResponseHttp>;
}