// Application/Features/SurchargeFeature/Dtos/SurchargeDTOs.cs
using Domain.Enums;

namespace Application.Features.SurchargeFeature.Dtos
{
    #region Surcharge DTOs

    public class SurchargeDTO
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Type { get; set; } = string.Empty;
        public string CalculationType { get; set; } = string.Empty;
        public decimal Value { get; set; }
        public bool IsActive { get; set; }
        public int RulesCount { get; set; }
    }

    public class SurchargeDetailsDTO
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Type { get; set; } = string.Empty;
        public string CalculationType { get; set; } = string.Empty;
        public decimal Value { get; set; }
        public bool IsActive { get; set; }
        public List<SurchargeRuleDTO> Rules { get; set; } = new();
    }

    public class CreateSurchargeDTO
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Type { get; set; } = string.Empty;
        public string CalculationType { get; set; } = string.Empty;
        public decimal Value { get; set; }
    }

    public class UpdateSurchargeDTO
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Type { get; set; } = string.Empty;
        public string CalculationType { get; set; } = string.Empty;
        public decimal Value { get; set; }
        public bool IsActive { get; set; }
    }

    #endregion

    #region SurchargeRule DTOs

    public class SurchargeRuleDTO
    {
        public Guid Id { get; set; }
        public Guid SurchargeId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ConditionsJson { get; set; } = "{}";
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
        public Guid? ZoneFromId { get; set; }
        public string? ZoneFromName { get; set; }
        public Guid? ZoneToId { get; set; }
        public string? ZoneToName { get; set; }
        public List<string>? ApplicableTransportModes { get; set; }
        public decimal? OverrideValue { get; set; }
        public int Priority { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateSurchargeRuleDTO
    {
        public string Name { get; set; } = string.Empty;
        public string ConditionsJson { get; set; } = "{}";
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
        public Guid? ZoneFromId { get; set; }
        public Guid? ZoneToId { get; set; }
        public List<string>? ApplicableTransportModes { get; set; }
        public decimal? OverrideValue { get; set; }
        public int Priority { get; set; } = 0;
    }

    public class UpdateSurchargeRuleDTO
    {
        public string Name { get; set; } = string.Empty;
        public string ConditionsJson { get; set; } = "{}";
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
        public Guid? ZoneFromId { get; set; }
        public Guid? ZoneToId { get; set; }
        public List<string>? ApplicableTransportModes { get; set; }
        public decimal? OverrideValue { get; set; }
        public int Priority { get; set; }
    }

    #endregion
}