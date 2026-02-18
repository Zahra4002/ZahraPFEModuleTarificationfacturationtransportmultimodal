using Domain.Common;
using Domain.Enums;

namespace Domain.Entities
{
    public class TransportSegment : Entity
    {
        public Guid? ShipmentId { get; set; }
        public int Sequence { get; set; }
        public TransportMode TransportMode { get; set; }
        public Guid? SupplierId { get; set; }

        public virtual Supplier? Supplier { get; set; }
        public Guid? ZoneFromId { get; set; }

        public virtual Zone ZoneFromet { get; set; }
        public Guid? ZoneToId { get; set; }

        public virtual Zone ZoneTo { get; set; }
        public decimal? DistanceKm { get; set; }
        public int? EstimatedTransitDays { get; set; }
        public DateTime? DepartureDate { get; set; }
        public DateTime? ArrivalDate { get; set; }
        public decimal BaseCost { get; set; }
        public decimal SurchargesTotal { get; set; }
        public decimal TotalCost { get; set; }
        public string CurrencyCode { get; set; }
    }
}
