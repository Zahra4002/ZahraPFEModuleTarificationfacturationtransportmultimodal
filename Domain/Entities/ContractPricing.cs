using Domain.Common;
using Domain.Enums;

namespace Domain.Entities
{
    /// <summary>
    /// Entité représentant un tarif spécifique dans un contrat
    /// </summary>
    public class ContractPricing : Entity
    {
        public Guid ContractId { get; set; }

        // Zones concernées
        public Guid? ZoneFromId { get; set; }
        public Guid? ZoneToId { get; set; }

        // Mode de transport
        public TransportMode? TransportMode { get; set; }

        // Type de tarification
        public bool UseFixedPrice { get; set; } = false;
        public decimal? FixedPrice { get; set; }
        public decimal DiscountPercent { get; set; } = 0;

        // Seuils de volume pour remises progressives
        public decimal? VolumeThreshold { get; set; }
        public decimal? VolumeDiscountPercent { get; set; }

        public string CurrencyCode { get; set; } = "EUR";
        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual Contract Contract { get; set; } = null!;
        public virtual Zone? ZoneFrom { get; set; }
        public virtual Zone? ZoneTo { get; set; }
    }
}
