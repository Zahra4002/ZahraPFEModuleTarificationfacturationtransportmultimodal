using Application.Setting;
using MediatR;
using System;

namespace Application.Features.QuoteFeature.Commands
{
    public record DeleteQuoteCommand(Guid Id) : IRequest<ResponseHttp>;
}