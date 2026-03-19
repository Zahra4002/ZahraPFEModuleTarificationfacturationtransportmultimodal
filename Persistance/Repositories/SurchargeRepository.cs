// Persistance/Repositories/SurchargeRepository.cs
using Application.Interfaces;
using Domain.Common;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Persistance.Data;
using System.Text.Json;

namespace Persistance.Repositories
{
    public class SurchargeRepository : GenericRepository<Surcharge>, ISurchargeRepository
    {
        private readonly CleanArchitecturContext _context;

        public SurchargeRepository(CleanArchitecturContext context) : base(context)
        {
            _context = context;
        }

        #region Surcharge Methods

        public async Task<PagedList<Surcharge>> GetAllWithFiltersAsync(
            int? pageNumber,
            int? pageSize,
            string? sortBy,
            bool sortDescending,
            string? searchTerm,
            SurchargeType? type,
            bool? isActive,
            CancellationToken cancellationToken)
        {
            IQueryable<Surcharge> query = _context.Set<Surcharge>()
                .Where(s => !s.IsDeleted);

            // Apply filters
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(s =>
                    s.Code.Contains(searchTerm) ||
                    s.Name.Contains(searchTerm) ||
                    (s.Description != null && s.Description.Contains(searchTerm)));
            }

            if (type.HasValue)
            {
                query = query.Where(s => s.Type == type.Value);
            }

            if (isActive.HasValue)
            {
                query = query.Where(s => s.IsActive == isActive.Value);
            }

            // Apply sorting
            if (!string.IsNullOrWhiteSpace(sortBy))
            {
                switch (sortBy.ToLower())
                {
                    case "code":
                        query = sortDescending
                            ? query.OrderByDescending(s => s.Code)
                            : query.OrderBy(s => s.Code);
                        break;
                    case "name":
                        query = sortDescending
                            ? query.OrderByDescending(s => s.Name)
                            : query.OrderBy(s => s.Name);
                        break;
                    case "type":
                        query = sortDescending
                            ? query.OrderByDescending(s => s.Type)
                            : query.OrderBy(s => s.Type);
                        break;
                    case "value":
                        query = sortDescending
                            ? query.OrderByDescending(s => s.Value)
                            : query.OrderBy(s => s.Value);
                        break;
                    default:
                        query = query.OrderBy(s => s.CreatedDate);
                        break;
                }
            }
            else
            {
                query = query.OrderBy(s => s.CreatedDate);
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var page = pageNumber ?? 1;
            var size = pageSize ?? 10;

            var items = await query
                .Skip((page - 1) * size)
                .Take(size)
                .ToListAsync(cancellationToken);

            return new PagedList<Surcharge>(items, totalCount, page, size);
        }

        public async Task<Surcharge?> GetByCodeAsync(string code)
        {
            return await _context.Set<Surcharge>()
                .FirstOrDefaultAsync(s => s.Code == code && !s.IsDeleted);
        }

        public async Task<Surcharge?> GetByIdWithRulesAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _context.Set<Surcharge>()
                .Include(s => s.Rules.Where(r => !r.IsDeleted))
                    .ThenInclude(r => r.ZoneFrom)
                .Include(s => s.Rules.Where(r => !r.IsDeleted))
                    .ThenInclude(r => r.ZoneTo)
                .FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted, cancellationToken);
        }

        public async Task<IEnumerable<Surcharge>> GetByTypeAsync(SurchargeType type, CancellationToken cancellationToken)
        {
            return await _context.Set<Surcharge>()
                .Where(s => s.Type == type && !s.IsDeleted && s.IsActive)
                .OrderBy(s => s.Code)
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> CodeExistsAsync(string code, Guid? excludeId = null)
        {
            var query = _context.Set<Surcharge>()
                .Where(s => s.Code == code && !s.IsDeleted);

            if (excludeId.HasValue)
            {
                query = query.Where(s => s.Id != excludeId.Value);
            }

            return await query.AnyAsync();
        }

        #endregion

        #region SurchargeRule Methods

        public async Task<SurchargeRule?> GetRuleByIdAsync(Guid ruleId, CancellationToken cancellationToken)
        {
            return await _context.Set<SurchargeRule>()
                .FirstOrDefaultAsync(r => r.Id == ruleId && !r.IsDeleted, cancellationToken);
        }

        public async Task<SurchargeRule?> GetRuleByIdWithDetailsAsync(Guid ruleId, CancellationToken cancellationToken)
        {
            return await _context.Set<SurchargeRule>()
                .Include(r => r.Surcharge)
                .Include(r => r.ZoneFrom)
                .Include(r => r.ZoneTo)
                .FirstOrDefaultAsync(r => r.Id == ruleId && !r.IsDeleted, cancellationToken);
        }

        public async Task<IEnumerable<SurchargeRule>> GetRulesBySurchargeIdAsync(Guid surchargeId, CancellationToken cancellationToken)
        {
            return await _context.Set<SurchargeRule>()
                .Include(r => r.ZoneFrom)
                .Include(r => r.ZoneTo)
                .Where(r => r.SurchargeId == surchargeId && !r.IsDeleted)
                .OrderByDescending(r => r.Priority)
                .ThenBy(r => r.Name)
                .ToListAsync(cancellationToken);
        }

        public async Task<SurchargeRule> AddRuleAsync(SurchargeRule rule)
        {
            await _context.Set<SurchargeRule>().AddAsync(rule);
            return rule;
        }

        public async Task UpdateRuleAsync(SurchargeRule rule)
        {
            _context.Set<SurchargeRule>().Update(rule);
            await Task.CompletedTask;
        }

        public async Task<bool> DeleteRuleAsync(Guid ruleId)
        {
            var rule = await _context.Set<SurchargeRule>().FindAsync(ruleId);
            if (rule == null)
                return false;

            if (rule is ISoftDeleteable softDeletableEntity)
            {
                softDeletableEntity.IsDeleted = true;
                softDeletableEntity.DeletedDate = DateTime.UtcNow;
                _context.Entry(rule).State = EntityState.Modified;
                return true;
            }

            return false;
        }

        public async Task<bool> RuleExistsInSurchargeAsync(Guid surchargeId, string ruleName, Guid? excludeRuleId = null)
        {
            var query = _context.Set<SurchargeRule>()
                .Where(r => r.SurchargeId == surchargeId &&
                           r.Name == ruleName &&
                           !r.IsDeleted);

            if (excludeRuleId.HasValue)
            {
                query = query.Where(r => r.Id != excludeRuleId.Value);
            }

            return await query.AnyAsync();
        }

        #endregion

        public async Task<List<Surcharge>> GetApplicableSurchargesAsync(TransportMode transportMode, DateTime date, Guid? zoneFromId, Guid? zoneToId, CancellationToken cancellationToken = default)
        {
            // Récupérer toutes les surcharges actives
            var query = _context.Surcharges
                .Include(s => s.Rules)
                .Where(s => s.IsActive && !s.IsDeleted);
            
            var surcharges = await query.ToListAsync(cancellationToken);
            
            // Filtrer en mémoire car les règles peuvent contenir des conditions complexes
            var applicableSurcharges = surcharges
                .Where(s => IsSurchargeApplicable(s, transportMode, date, zoneFromId, zoneToId))
                .ToList();
            
            return applicableSurcharges;
        }

        private bool IsSurchargeApplicable(Surcharge surcharge, TransportMode transportMode, DateTime date, Guid? zoneFromId, Guid? zoneToId)
        {
            // TODO: Implement your business logic here
            // Example implementation:
            if (!surcharge.Rules.Any())
                return true;
            
            return surcharge.Rules.Any(rule => 
                !rule.IsDeleted && 
                // Add your rule evaluation logic here
                true);
        }
    }
}