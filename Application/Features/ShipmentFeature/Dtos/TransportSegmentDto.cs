using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Enums;

namespace Application.Features.ShipmentFeature.Dtos
{
    public  class TransportSegmentDto
    {
        public Guid Id { get; set; }
        public Guid? ShipmentId { get; set; }
        public int Sequence { get; set; }
        public TransportMode TransportMode { get; set; }
        public Guid? SupplierId { get; set; }
        public Guid? ZoneFromId { get; set; }
        public Guid? ZoneToId { get; set; }
        public decimal? DistanceKm { get; set; }
        public int? EstimatedTransitDays { get; set; }
        public DateTime? DepartureDate { get; set; }
        public DateTime? ArrivalDate { get; set; }
        public decimal BaseCost { get; set; }
        public decimal SurchargesTotal { get; set; }
        public decimal TotalCost { get; set; }
        public string CurrencyCode { get; set; }

        public TransportSegmentDto(TransportSegment ts)
        {
            Id = ts.Id;
            ShipmentId = ts.ShipmentId;
            Sequence = ts.Sequence;
            TransportMode = ts.TransportMode;
            SupplierId = ts.SupplierId;
            ZoneFromId = ts.ZoneFromId;
            ZoneToId = ts.ZoneToId;
            DistanceKm = ts.DistanceKm;
            EstimatedTransitDays = ts.EstimatedTransitDays;
            DepartureDate = ts.DepartureDate;
            ArrivalDate = ts.ArrivalDate;
            BaseCost = ts.BaseCost;
            SurchargesTotal = ts.SurchargesTotal;
            TotalCost = ts.TotalCost;
            CurrencyCode = ts.CurrencyCode;
        }


    }
}
