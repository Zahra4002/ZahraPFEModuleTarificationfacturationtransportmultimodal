using Domain.ValueObjects;
using System;

namespace Application.Features.QuoteFeature.Dtos
{
    public class CreateQuoteDto
    {
        public string QuoteNumber { get; set; } = string.Empty;
        public Guid? ClientId { get; set; }

        public Address OriginAddress { get; set; } = new();
        public Address DestinationAddress { get; set; } = new();

        public decimal? WeightKg { get; set; }
        public decimal? VolumeM3 { get; set; }
        public Guid? MerchandiseTypeId { get; set; }

        public decimal TotalHT { get; set; }
        public decimal TotalTTC { get; set; }
        public string CurrencyCode { get; set; } = "EUR";

        public DateTime ValidUntil { get; set; }
        public string? Notes { get; set; }
    }
}