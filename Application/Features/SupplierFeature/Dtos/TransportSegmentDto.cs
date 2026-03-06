using System;

namespace Application.Features.SupplierFeature.Dtos
{
    public class TransportSegmentDto
    {
        public Guid Id { get; set; }
        public int Sequence { get; set; }
        public int TransportMode { get; set; }
        public string TransportModeName { get; set; } = string.Empty;
        public Guid? ZoneFromId { get; set; }
        public string? ZoneFromName { get; set; }
        public Guid? ZoneToId { get; set; }
        public string? ZoneToName { get; set; }
        public decimal? DistanceKm { get; set; }
        public decimal BaseCost { get; set; }
        public string CurrencyCode { get; set; } = string.Empty;
        public decimal TotalCost { get; set; }
        public decimal SurchargesTotal { get; set; }
    }
}