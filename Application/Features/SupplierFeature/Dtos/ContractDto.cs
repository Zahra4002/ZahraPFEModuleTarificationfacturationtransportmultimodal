using System;

namespace Application.Features.SupplierFeature.Dtos
{
    public class ContractDto
    {
        internal readonly object SupplierName;

        public Guid Id { get; set; }
        public string ContractNumber { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int Type { get; set; }
        public Guid? ClientId { get; set; }
        public string? ClientName { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public string? Terms { get; set; }
        public bool TermsAccepted { get; set; }
        public DateTime? TermsAcceptedAt { get; set; }
        public decimal GlobalDiscountPercent { get; set; }
        public decimal? MinimumVolume { get; set; }
        public string? MinimumVolumeUnit { get; set; }
        public bool IsActive { get; set; }
        public bool AutoRenew { get; set; }
        public int? RenewalNoticeDays { get; set; }
        public bool IsValid { get; set; }
        public bool IsExpiringSoon { get; set; }
    }
}