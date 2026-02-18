using Domain.Common;

namespace Domain.Entities
{
    public class SurchargeRule : Entity
    {
        public Guid? SurchargeId { get; set; }
        public string Name { get; set; } = string.Empty;

        // Conditions d'application (stockées en JSON)
        public string ConditionsJson { get; set; } = "{}";

        // Période de validité
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }

        // Zones applicables (null = toutes)
        public Guid? ZoneFromId { get; set; }
        public Guid? ZoneToId { get; set; }

        // Modes de transport applicables (null = tous)
        public string? ApplicableTransportModes { get; set; } // JSON array

        // Valeur spécifique si différente de la surcharge parente
        public decimal? OverrideValue { get; set; }

        public int Priority { get; set; } = 0; // Plus élevé = prioritaire
        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual Surcharge Surcharge { get; set; } = null!;
        public virtual Zone? ZoneFrom { get; set; }
        public virtual Zone? ZoneTo { get; set; }

        public bool IsValidAt(DateTime date) =>
            (!ValidFrom.HasValue || date >= ValidFrom.Value) &&
            (!ValidTo.HasValue || date <= ValidTo.Value);
    }
}
