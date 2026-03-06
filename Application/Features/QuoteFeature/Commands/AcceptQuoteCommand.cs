using Application.Setting;
using MediatR;
using System;

namespace Application.Features.QuoteFeature.Commands
{
    public record AcceptQuoteCommand(Guid Id) : IRequest<ResponseHttp>;
}