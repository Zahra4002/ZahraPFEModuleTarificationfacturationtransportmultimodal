using Domain.Common;
using Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;


namespace Domain.Entities
{
    public class InvoiceLine : Entity
    {
        public readonly object TransportSegment;

        public Guid InvoiceId { get; set; }
        public virtual Invoice? Invoice { get; set; }

        public string? Description { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPriceHT { get; set; }
        public decimal VATRate { get; set; }
        public decimal DiscountPercent { get; set; } = 0;
        public Guid? TransportSegmentId { get; set; }
        
        //public virtual TransportSegment TransportSegment { get; set; }

        // ✅ Propriétés calculées - Pas de colonne en base
        [NotMapped]
        public decimal TotalHT => Quantity * UnitPriceHT;

        [NotMapped]
        public decimal TotalVAT => TotalHT * (VATRate / 100);

        [NotMapped]
        public decimal TotalTTC => TotalHT + TotalVAT;
    }
}
