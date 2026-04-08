using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistance.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Persistance.Repositories
{
    public class TaxRuleRepository : GenericRepository<TaxRule>, ITaxRuleRepository
    {
        private readonly CleanArchitecturContext _context;

        public TaxRuleRepository(CleanArchitecturContext context) : base(context)
        {
            _context = context;
        }

        public async Task<TaxRule?> GetApplicableTaxRuleAsync(string country, DateTime date, CancellationToken cancellationToken = default)
        {
            return await _context.TaxRules
                .Where(t => t.Country == country
                    && t.IsActive
                    && !t.IsDeleted
                    && t.ValidFrom <= date
                    && (t.ValidTo == null || t.ValidTo >= date))
                .OrderByDescending(t => t.ValidFrom)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<TaxRule?> GetApplicableTaxRuleAsync(string country, string? region, DateTime date, CancellationToken cancellationToken = default)
        {
            var query = _context.TaxRules
                .Where(t => t.Country == country
                    && t.IsActive
                    && !t.IsDeleted
                    && t.ValidFrom <= date
                    && (t.ValidTo == null || t.ValidTo >= date));

            if (!string.IsNullOrEmpty(region))
            {
                var regionSpecificRule = await query
                    .Where(t => t.Region == region)
                    .OrderByDescending(t => t.ValidFrom)
                    .FirstOrDefaultAsync(cancellationToken);

                if (regionSpecificRule != null)
                    return regionSpecificRule;
            }

            return await query
                .Where(t => t.Region == null)
                .OrderByDescending(t => t.ValidFrom)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<List<TaxRule>> GetActiveTaxRulesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.TaxRules
                .Where(t => t.IsActive && !t.IsDeleted)
                .OrderBy(t => t.Country)
                .ThenBy(t => t.Region)
                .ThenByDescending(t => t.ValidFrom)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<TaxRule>> GetTaxRulesByCountryAsync(string country, CancellationToken cancellationToken = default)
        {
            return await _context.TaxRules
                .Where(t => t.Country == country && !t.IsDeleted)
                .OrderByDescending(t => t.ValidFrom)
                .ToListAsync(cancellationToken);
        }

        public async Task<TaxRule?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
        {
            return await _context.TaxRules
                .FirstOrDefaultAsync(t => t.Code == code && !t.IsDeleted, cancellationToken);
        }

        /// <summary>
        /// Récupère une règle de taxe par son code pays (FR, TN, etc.)
        /// </summary>
        public async Task<TaxRule?> GetByCountryCode(string countryCode, CancellationToken cancellationToken = default)
        {
            return await _context.TaxRules
                .FirstOrDefaultAsync(t => t.Code == countryCode && !t.IsDeleted, cancellationToken);
        }
    }
}