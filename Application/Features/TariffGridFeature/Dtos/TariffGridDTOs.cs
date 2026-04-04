// Application/Features/TariffGridFeature/Dtos/TariffGridDTOs.cs
using Domain.Enums;

namespace Application.Features.TariffGridFeature.Dtos
{
    #region Tariff Grid DTOs

    public class TariffGridDTO
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string TransportMode { get; set; } = string.Empty;
        public int Version { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
        public bool IsActive { get; set; }
        public string CurrencyCode { get; set; } = string.Empty;

        // ✅ Cette propriété doit être calculée
        public int TariffLinesCount { get; set; }  // Compte des TariffLines

        public DateTime CreatedAt { get; set; }
    }

    public class TariffGridDetailsDTO
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string TransportMode { get; set; } = string.Empty;
        public int Version { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
        public bool IsActive { get; set; }
        public string CurrencyCode { get; set; } = string.Empty;
        public List<TariffLineDTO> TariffLines { get; set; } = new();
        public DateTime CreatedAt { get; set; }
    }

    public class CreateTariffGridDTO
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string TransportMode { get; set; } = string.Empty;
        public DateTime ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
        public string CurrencyCode { get; set; } = string.Empty;
    }

    public class UpdateTariffGridDTO
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string TransportMode { get; set; } = string.Empty;
        public DateTime ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
        public bool IsActive { get; set; }
        public string CurrencyCode { get; set; } = string.Empty;
    }

    public class TariffGridHistoryDTO
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string TransportMode { get; set; } = string.Empty;
        public int Version { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
        public bool IsActive { get; set; }
        public string CurrencyCode { get; set; } = string.Empty;
        public int TariffLinesCount { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    #endregion

    #region Tariff Line DTOs

    public class TariffLineDTO
    {
        public Guid Id { get; set; }
        public Guid TariffGridId { get; set; }
        public Guid? ZoneFromId { get; set; }
        public string? ZoneFromName { get; set; }
        public Guid? ZoneToId { get; set; }
        public string? ZoneToName { get; set; }

        // Weight based pricing
        public decimal? PricePerKg { get; set; }
        public decimal? MinWeight { get; set; }
        public decimal? MaxWeight { get; set; }

        // Volume based pricing
        public decimal? PricePerM3 { get; set; }
        public decimal? MinVolume { get; set; }
        public decimal? MaxVolume { get; set; }

        // Container based pricing
        public decimal? PricePerContainer20ft { get; set; }
        public decimal? PricePerContainer40ft { get; set; }
        public decimal? PricePerContainer40ftHC { get; set; }
        public decimal? PricePerContainer60ftHC { get; set; }

        // Linear meter pricing
        public decimal? PricePerLinearMeter { get; set; }

        // General
        public decimal? BasePrice { get; set; }
        public int? TransitDays { get; set; }
        public bool IsActive { get; set; }

        // Height based pricing (for certain modes)
        public decimal? MinHeight { get; set; }
        public decimal? MaxHeight { get; set; }
    }

    public class CreateTariffLineDTO
    {
        public Guid? ZoneFromId { get; set; }
        public Guid? ZoneToId { get; set; }

        // Weight based pricing
        public decimal? PricePerKg { get; set; }
        public decimal? MinWeight { get; set; }
        public decimal? MaxWeight { get; set; }

        // Volume based pricing
        public decimal? PricePerM3 { get; set; }
        public decimal? MinVolume { get; set; }
        public decimal? MaxVolume { get; set; }

        // Container based pricing
        public decimal? PricePerContainer20ft { get; set; }
        public decimal? PricePerContainer40ft { get; set; }
        public decimal? PricePerContainer40ftHC { get; set; }
        public decimal? PricePerContainer60ftHC { get; set; }

        // Linear meter pricing
        public decimal? PricePerLinearMeter { get; set; }

        // General
        public decimal? BasePrice { get; set; }
        public int? TransitDays { get; set; }

        // Height based pricing
        public decimal? MinHeight { get; set; }
        public decimal? MaxHeight { get; set; }
    }

    public class CreateTariffLineBulkDTO
    {
        public List<CreateTariffLineDTO> Lines { get; set; } = new();
    }

    public class UpdateTariffLineDTO
    {
        public Guid? ZoneFromId { get; set; }
        public Guid? ZoneToId { get; set; }

        // Weight based pricing
        public decimal? PricePerKg { get; set; }
        public decimal? MinWeight { get; set; }
        public decimal? MaxWeight { get; set; }

        // Volume based pricing
        public decimal? PricePerM3 { get; set; }
        public decimal? MinVolume { get; set; }
        public decimal? MaxVolume { get; set; }

        // Container based pricing
        public decimal? PricePerContainer20ft { get; set; }
        public decimal? PricePerContainer40ft { get; set; }
        public decimal? PricePerContainer40ftHC { get; set; }

        // Linear meter pricing
        public decimal? PricePerLinearMeter { get; set; }

        // General
        public decimal? BasePrice { get; set; }
        public int? TransitDays { get; set; }
    }

    public class TariffGridListResponseDTO
    {
        public List<TariffGridDTO> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public bool HasPreviousPage { get; set; }
        public bool HasNextPage { get; set; }
    }

    #endregion
}