using Domain.Common;
using Domain.Enums;

namespace Domain.Entities
{
    public class TariffGrid : Entity
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public int Version { get; set; }
        public TransportMode TransportMode { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
        public string CurrencyCode { get; set; } = "EUR";
        public bool IsActive { get; set; }
        public string? Description { get; set; }

        public virtual ICollection<TariffLine> TariffLines { get; set; }

        public bool IsValidAt(DateTime date)
        {
            return date >= ValidFrom && (!ValidTo.HasValue || date <= ValidTo.Value);
        }
    }
}
