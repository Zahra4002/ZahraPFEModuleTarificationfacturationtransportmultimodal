using Domain.Common;

namespace Domain.Entities
{
    public class SegmentCost : Entity
    {
        public Guid TransportSegmentId { get; set; }
        public string CostType { get; set; } = string.Empty; // "BASE", "SURCHARGE", "TAX"
        public string Description { get; set; } = string.Empty;
        public Guid? SurchargeId { get; set; }

        public decimal Amount { get; set; }
        public string CurrencyCode { get; set; } = "EUR";

        // Pour traçabilité
        public string? CalculationBasis { get; set; } // Ex: "150 kg x 2.50 EUR/kg"

        // Navigation properties
        public virtual TransportSegment TransportSegment { get; set; } = null!;
        public virtual Surcharge Surcharge { get; set; }
    }
}
