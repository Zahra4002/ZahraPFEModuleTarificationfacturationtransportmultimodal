using System;
using Domain.Entities;
using Domain.Enums;

namespace Application.Features.ContractFeature.Dtos
{
    public class ContractPricingDto
    {
        public Guid Id { get; set; }
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

        // Propriétés d'audit (héritées de Entity)
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? CreatedBy { get; set; }
        public string? ModifiedBy { get; set; }

        // Propriétés optionnelles pour les noms des entités liées
        public string? ZoneFromName { get; set; }
        public string? ZoneToName { get; set; }
        public string? TransportModeDisplayName { get; set; }


       
    }
}
