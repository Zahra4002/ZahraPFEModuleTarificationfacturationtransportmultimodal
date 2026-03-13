using Domain.Common;
using Domain.Enums;
using Domain.ValueObjects;

namespace Domain.Entities
{
    public class Shipment : Entity
    {
        public string ShipmentNumber { get; set; }
        public Guid? ClientId { get; set; }

        /// <summary>
        /// Gets or sets the client associated with the current operation.
        /// </summary>
        public virtual Client Client { get; set; }
        public Guid? QuoteId { get; set; }

        public virtual Quote? Quote { get; set; }
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
        // Make sure CurrencyCode is never null to avoid NOT NULL constraint violations in the database.
        public string CurrencyCode { get; set; } = string.Empty;

        // Initialize collections to avoid null reference issues when mapping or adding segments/invoices.
        public virtual ICollection<TransportSegment> Segments { get; set; } = new List<TransportSegment>();
        public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
         public decimal CalculateTotalCostHT()
        {
            if (Segments == null || !Segments.Any())
                return 0m;

            return Segments.Sum(s => s.BaseCost);
        }

        public decimal CalculateTotalSurcharges()
        {
            if (Segments == null || !Segments.Any())
                return 0m;

            return Segments.Sum(s => s.SurchargesTotal);
        }

        public decimal CalculateTotalTaxes()
        {
            if (Invoices == null || !Invoices.Any())
                return 0m;

            return Invoices.Sum(i => i.TotalVAT);
        }

        public decimal CalculateTotalCostTTC()
        {
            return CalculateTotalCostHT()
                 + CalculateTotalSurcharges()
                 + CalculateTotalTaxes();
        }

        public void RecalculateTotals()
        {
            TotalCostHT = CalculateTotalCostHT();
            TotalSurcharges = CalculateTotalSurcharges();
            TotalTaxes = CalculateTotalTaxes();
            TotalCostTTC = CalculateTotalCostTTC();
        }
    }

   
}
