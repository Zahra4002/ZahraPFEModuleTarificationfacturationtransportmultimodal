using Application.Setting;
using Domain.ValueObjects;
using MediatR;
using System;

namespace Application.Features.QuoteFeature.Commands
{
    public record UpdateQuoteCommand(
        Guid Id,
        string? QuoteNumber,
        Guid? ClientId,
        Address? OriginAddress,
        Address? DestinationAddress,
        decimal? WeightKg,
        decimal? VolumeM3,
        Guid? MerchandiseTypeId,
        decimal? TotalHT,
        decimal? TotalTTC,
        string? CurrencyCode,
        DateTime? ValidUntil,
        bool? IsAccepted,
        DateTime? AcceptedAt,
        string? Notes
    ) : IRequest<ResponseHttp>;
}