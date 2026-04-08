using System;

namespace Application.Features.MerchandiseTypeFeature.Dtos
{
    public class MerchandiseTypeDto
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int HazardousLevel { get; set; }
        public decimal? PriceMultiplier { get; set; }
        public bool RequiresSpecialHandling { get; set; }
        public bool IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }      // ← Note: CreatedDate
        public DateTime? ModifiedDate { get; set; }     // ← Note: ModifiedDate
    }
}