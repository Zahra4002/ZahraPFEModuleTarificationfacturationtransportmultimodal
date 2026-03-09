using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.ShipmentFeature.Dtos
{
    public class ShipmentDto
    {
        public Guid Id { get; set; }
        public string ShipmentNumber { get; set; }
        public Guid? ClientId { get; set; }
        public Guid? QuoteId { get; set; }
        public Address OriginAddress { get; set; }
        public Address DestinationAddress { get; set; }
        public Guid? MerchandiseTypeId { get; set; }
        public decimal? WeightKg { get; set; }
        public decimal? VolumeM3 { get; set; }
        public ContainerType? ContainerType { get; set; }
        public int? ContainerCount { get; set; }
        public ShipmentStatus Status { get; set; }
        public string? TrackingNumber { get; set; }
        public decimal TotalCostHT { get; set; }
        public decimal TotalSurcharges { get; set; }
        public decimal TotalTaxes { get; set; }
        public decimal TotalCostTTC { get; set; }
        public string CurrencyCode { get; set; }

        public List<TransportSegmentDto> TransportSegments { get; set; }

        public ShipmentDto(Shipment shipment)
        {
            Id = shipment.Id;
            ShipmentNumber = shipment.ShipmentNumber;
            ClientId = shipment.ClientId;
            QuoteId = shipment.QuoteId;
            OriginAddress = shipment.OriginAddress;
            DestinationAddress = shipment.DestinationAddress;
            MerchandiseTypeId = shipment.MerchandiseTypeId;
            WeightKg = shipment.WeightKg;
            VolumeM3 = shipment.VolumeM3;
            ContainerType = shipment.ContainerType;
            ContainerCount = shipment.ContainerCount;
            Status = shipment.Status;
            TrackingNumber = shipment.TrackingNumber;
            TotalCostHT = shipment.TotalCostHT;
            TotalSurcharges = shipment.TotalSurcharges;
            TotalTaxes = shipment.TotalTaxes;
            TotalCostTTC = shipment.TotalCostTTC;
            CurrencyCode = shipment.CurrencyCode;
            TransportSegments = shipment.Segments?.Select(ts => new TransportSegmentDto(ts)).ToList() ?? new List<TransportSegmentDto>();

        }
    }
}