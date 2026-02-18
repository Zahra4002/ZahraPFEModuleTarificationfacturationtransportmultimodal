using Domain.Common;

namespace Domain.Entities
{
    public class Zone : Entity
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string? Region { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
        // Navigation properties
        public virtual ICollection<TariffLine> TariffLinesFrom { get; set; } = new List<TariffLine>();
        public virtual ICollection<TariffLine> TariffLinesTo { get; set; } = new List<TariffLine>();

    }
}
