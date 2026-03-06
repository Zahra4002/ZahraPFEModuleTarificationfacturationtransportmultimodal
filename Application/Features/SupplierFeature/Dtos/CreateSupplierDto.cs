using System.Collections.Generic;

namespace Application.Features.SupplierFeature.Dtos
{
    public class CreateSupplierDto
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? TaxId { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string Address { get; set; } = string.Empty;
        public string DefaultCurrencyCode { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;

        // ✅ Collections optionnelles
        public List<CreateContractDto>? Contracts { get; set; }
        public List<CreateTransportSegmentDto>? TransportSegments { get; set; }
    }

    public class CreateContractDto
    {
        public string ContractNumber { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int Type { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public string? Terms { get; set; }
        public decimal GlobalDiscountPercent { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class CreateTransportSegmentDto
    {
        public int Sequence { get; set; }
        public int TransportMode { get; set; }
        public Guid? ZoneFromId { get; set; }
        public Guid? ZoneToId { get; set; }
        public decimal? DistanceKm { get; set; }
        public decimal BaseCost { get; set; }
        public string CurrencyCode { get; set; } = "EUR";
    }
}
