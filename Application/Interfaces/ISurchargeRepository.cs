// Application/Interfaces/ISurchargeRepository.cs
using Domain.Common;
using Domain.Entities;
using Domain.Enums;

namespace Application.Interfaces
{
    public interface ISurchargeRepository : IGenericRepository<Surcharge>
    {
        // Surcharge methods
        Task<PagedList<Surcharge>> GetAllWithFiltersAsync(
            int? pageNumber,
            int? pageSize,
            string? sortBy,
            bool sortDescending,
            string? searchTerm,
            SurchargeType? type,
            bool? isActive,
            CancellationToken cancellationToken);

        Task<Surcharge?> GetByCodeAsync(string code);
        Task<Surcharge?> GetByIdWithRulesAsync(Guid id, CancellationToken cancellationToken);
        Task<IEnumerable<Surcharge>> GetByTypeAsync(SurchargeType type, CancellationToken cancellationToken);
        Task<bool> CodeExistsAsync(string code, Guid? excludeId = null);

        // SurchargeRule methods
        Task<SurchargeRule?> GetRuleByIdAsync(Guid ruleId, CancellationToken cancellationToken);
        Task<SurchargeRule?> GetRuleByIdWithDetailsAsync(Guid ruleId, CancellationToken cancellationToken);
        Task<IEnumerable<SurchargeRule>> GetRulesBySurchargeIdAsync(Guid surchargeId, CancellationToken cancellationToken);
        Task<SurchargeRule> AddRuleAsync(SurchargeRule rule);
        Task UpdateRuleAsync(SurchargeRule rule);
        Task<bool> DeleteRuleAsync(Guid ruleId);
        Task<bool> RuleExistsInSurchargeAsync(Guid surchargeId, string ruleName, Guid? excludeRuleId = null);

            Task<List<Surcharge>> GetApplicableSurchargesAsync(
                  TransportMode transportMode,
                  DateTime date,
                  Guid? zoneFromId,
                  Guid? zoneToId,
                  CancellationToken cancellationToken = default);
    }
}