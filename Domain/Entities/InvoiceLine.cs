using Domain.Common;
using Domain.Enums;


namespace Domain.Entities
{
    public class InvoiceLine: Entity
    {
        public Guid InvoiceId { get; set; }
        public virtual Invoice Invoice { get; set; }

        public string? Description { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPriceHT { get; set; }
        public decimal DiscountPercent { get; set; }
        public decimal VATRate { get; set; }

        public InvoiceLineType LineType { get; set; }
        public Guid? TransportSegmentId { get; set; }
        public Guid? SurchargeId { get; set; }

        // Computed properties
        public decimal TotalHT => Quantity * UnitPriceHT * (1 - DiscountPercent / 100);
        public decimal TotalVAT => TotalHT * VATRate / 100;

    }
}
