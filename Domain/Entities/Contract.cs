using Domain.Common;
using Domain.Enums;

namespace Domain.Entities
{
    public class Contract : Entity
    {
        public string ContractNumber { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public ContractType Type { get; set; }

        // Partie contractante
        public Guid? ClientId { get; set; }
        public Guid? SupplierId { get; set; }

        // P�riode de validit�
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }

        // Conditions g�n�rales
        public string? Terms { get; set; }
        public bool TermsAccepted { get; set; } = false;
        public DateTime? TermsAcceptedAt { get; set; }

        // Remise globale
        public decimal GlobalDiscountPercent { get; set; } = 0;

        // Volume minimum engag�
        public decimal? MinimumVolume { get; set; }
        public string? MinimumVolumeUnit { get; set; } // ex: "TEU/an", "tonnes/mois"

        public bool IsActive { get; set; } = true;
        public bool AutoRenew { get; set; } = false;
        public int? RenewalNoticeDays { get; set; } = 30;

        // Navigation properties
        public virtual Client? Client { get; set; }
        public virtual Supplier? Supplier { get; set; }
        public virtual ICollection<ContractPricing> ContractPricings { get; set; } = new List<ContractPricing>();

        public bool IsValidAt(DateTime date) => date >= ValidFrom && date <= ValidTo && IsActive;

        public bool IsExpiringSoon(int days = 30) =>
            IsActive && ValidTo <= DateTime.UtcNow.AddDays(days);
    }
}
