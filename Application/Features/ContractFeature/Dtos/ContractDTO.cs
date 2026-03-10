using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Enums;

namespace Application.Features.ContractFeature.Dtos
{
    public class ContractDTO
    {
        public Guid Id { get; set; }
        public string ContractNumber { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public ContractType Type { get; set; }

        // Partie contractante
        public Guid? ClientId { get; set; }
        public Guid? SupplierId { get; set; }

        // Période de validité
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }

        // Conditions générales
        public string? Terms { get; set; }
        public bool TermsAccepted { get; set; } = false;
        public DateTime? TermsAcceptedAt { get; set; }

        // Remise globale
        public decimal GlobalDiscountPercent { get; set; } = 0;

        // Volume minimum engagé
        public decimal? MinimumVolume { get; set; }
        public string? MinimumVolumeUnit { get; set; } // ex: "TEU/an", "tonnes/mois"

        public bool IsActive { get; set; } = true;
        public bool AutoRenew { get; set; } = false;
        public int? RenewalNoticeDays { get; set; } = 30;

        
        
    }
}
