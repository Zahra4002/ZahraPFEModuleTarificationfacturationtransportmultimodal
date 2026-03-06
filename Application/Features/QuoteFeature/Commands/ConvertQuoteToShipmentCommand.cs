using Application.Setting;
using MediatR;
using System;

namespace Application.Features.QuoteFeature.Commands
{
    public record ConvertQuoteToShipmentCommand(Guid Id) : IRequest<ResponseHttp>;
}