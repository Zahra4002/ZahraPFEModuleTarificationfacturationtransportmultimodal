using Domain.ValueObjects;
using System;

namespace Application.Features.QuoteFeature.Dtos
{
    public class QuoteDto
    {
        public Guid Id { get; set; }
        public string QuoteNumber { get; set; } = string.Empty;
        public Guid? ClientId { get; set; }
        public string ClientName { get; set; } = string.Empty;

        // Adresses
        public Address OriginAddress { get; set; } = new();
        public Address DestinationAddress { get; set; } = new();

        public decimal? WeightKg { get; set; }
        public decimal? VolumeM3 { get; set; }
        public Guid? MerchandiseTypeId { get; set; }
        public string? MerchandiseTypeName { get; set; }
        public string? MerchandiseTypeCode { get; set; }
        // Tarification
        public decimal TotalHT { get; set; }
        public decimal TotalTTC { get; set; }
        public string CurrencyCode { get; set; } = "EUR";

        // Validité
        public DateTime ValidUntil { get; set; }
        public bool IsAccepted { get; set; }
        public DateTime? AcceptedAt { get; set; }
        public bool IsValid { get; set; }

        public string? Notes { get; set; }

        // Dates d'audit
        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
    }
}