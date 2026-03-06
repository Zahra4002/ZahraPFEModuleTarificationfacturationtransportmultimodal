using System.Collections.Generic;

namespace Application.Features.SupplierFeature.Dtos
{
    public class UpdateSupplierDto
    {
        public string? Code { get; set; }
        public string? Name { get; set; }
        public string? TaxId { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? DefaultCurrencyCode { get; set; }
        public bool? IsActive { get; set; }

        // ✅ Collections optionnelles
        public List<UpdateContractDto>? Contracts { get; set; }
        public List<UpdateTransportSegmentDto>? TransportSegments { get; set; }
    }

    public class UpdateContractDto
    {
        public string? ContractNumber { get; set; }
        public string? Name { get; set; }
        public int? Type { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
        public string? Terms { get; set; }
        public decimal? GlobalDiscountPercent { get; set; }
        public bool? IsActive { get; set; }
    }

    public class UpdateTransportSegmentDto
    {
        public int? Sequence { get; set; }
        public int? TransportMode { get; set; }
        public Guid? ZoneFromId { get; set; }
        public Guid? ZoneToId { get; set; }
        public decimal? DistanceKm { get; set; }
        public decimal? BaseCost { get; set; }
        public string? CurrencyCode { get; set; }
    }
}