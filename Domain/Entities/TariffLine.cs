using Domain.Common;

namespace Domain.Entities
{
    public class TariffLine : Entity
    {
        public Guid? TariffGridId { get; set; }
        public Guid? ZoneFromId { get; set; }
        public virtual Zone ZoneFrom { get; set; }
        public Guid? ZoneToId { get; set; }
        public virtual Zone ZoneTo { get; set; }
        public Guid? MerchandiseTypeId { get; set; }
        public decimal? MinWeight { get; set; }
        public decimal? MaxWeight { get; set; }
        public decimal? MinVolume { get; set; }
        public decimal? MaxVolume { get; set; }
        public decimal? PricePerKg { get; set; }
        public decimal? PricePerM3 { get; set; }
        public decimal? PricePerContainer20ft { get; set; }
        public decimal? PricePerContainer40ft { get; set; }
        public decimal? BasePrice { get; set; }
        public int? TransitDays { get; set; }
        public virtual TariffGrid TariffGrid { get; set; } = null!;

    }
}
