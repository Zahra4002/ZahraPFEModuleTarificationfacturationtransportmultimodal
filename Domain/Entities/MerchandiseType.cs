using Domain.Common;

namespace Domain.Entities
{
    public class MerchandiseType : Entity
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int HazardousLevel { get; set; } = 0; // 0 = non dangereux, 1-9 = classes ADR
        public decimal? PriceMultiplier { get; set; } = 1.0m;
        public bool RequiresSpecialHandling { get; set; } = false;
        public bool IsActive { get; set; } = true;

        public virtual ICollection<Quote> Quotes { get; set; } = new List<Quote>();

        public virtual ICollection<Shipment> Shipments { get; set; } = new List<Shipment>();



    }
}
