using Domain.ValueObjects;
using System;

namespace Application.Features.QuoteFeature.Dtos
{
    public class ConvertedShipmentDto
    {
        public Guid Id { get; set; }
        public string ShipmentNumber { get; set; } = string.Empty;
        public Guid? ClientId { get; set; }
        public string? ClientName { get; set; }
        public Address OriginAddress { get; set; } = new();
        public Address DestinationAddress { get; set; } = new();
        public string? GoodsDescription { get; set; }
        public decimal? WeightKg { get; set; }
        public decimal? VolumeM3 { get; set; }
        public int? NumberOfPackages { get; set; }
        public string Status { get; set; } = "Draft";
        public DateTime? RequestedPickupDate { get; set; }
    }
}