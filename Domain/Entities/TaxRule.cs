using Domain.Common;

namespace Domain.Entities
{
    /// <summary>
    /// Entité représentant une règle fiscale (TVA)
    /// </summary>
    public class TaxRule : Entity
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string? CountryCode { get; set; } = string.Empty;  // ← AJOUTER
        public string? Region { get; set; }

        public decimal StandardRate { get; set; } // Taux standard en %
        public decimal? ReducedRate { get; set; } // Taux réduit
        public decimal? SuperReducedRate { get; set; } // Taux super réduit
        public decimal? ZeroRate { get; set; } = 0; // Taux zéro

        // Conditions d'exonération
        public bool AllowExemption { get; set; } = false;
        public string? ExemptionConditions { get; set; } // JSON avec conditions

        public Surcharge? Surcharge { get; set; }
        public Guid? SurchargeId { get; set; }

        public DateTime ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
        public bool IsActive { get; set; } = true;

        public bool IsValidAt(DateTime date) =>
            date >= ValidFrom && (!ValidTo.HasValue || date <= ValidTo.Value);
    }
}