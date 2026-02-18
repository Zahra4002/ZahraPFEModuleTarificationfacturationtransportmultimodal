using Domain.Common;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class SpotPricing : Entity
    {
        public string Reference { get; set; } = string.Empty;
        public Guid ClientId { get; set; }
        public Guid ZoneFromId { get; set; }
        public Guid ZoneToId { get; set; }
        public TransportMode TransportMode { get; set; }

        // Prix spot
        public decimal Price { get; set; }
        public string CurrencyCode { get; set; } = "EUR";

        // Validité
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }

        public string? Notes { get; set; }
        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual Client Client { get; set; } = null!;
        public virtual Zone ZoneFrom { get; set; } = null!;
        public virtual Zone ZoneTo { get; set; } = null!;

        public bool IsValidAt(DateTime date) =>
            date >= ValidFrom && date <= ValidTo && IsActive;
    }
}
