using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistance.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        /// <summary>
        /// Récupère la règle de taxe applicable pour un pays et une date donnés
        /// </summary>
        public async Task<TaxRule?> GetApplicableTaxRuleAsync(string country, DateTime date, CancellationToken cancellationToken = default)
        {
            return await _context.TaxRules
                .Where(t => t.Country == country
&& t.IsActive
&& !t.IsDeleted
&& t.ValidFrom <= date
&& (t.ValidTo == null || t.ValidTo >= date))
                .OrderByDescending(t => t.ValidFrom) // Prend la plus récente
                .FirstOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        /// Récupère la règle de taxe applicable pour un pays, une région et une date donnés
        /// </summary>
        public async Task<TaxRule?> GetApplicableTaxRuleAsync(string country, string? region, DateTime date, CancellationToken cancellationToken = default)
        {
            var query = _context.TaxRules
                .Where(t => t.Country == country
&& t.IsActive
&& !t.IsDeleted
&& t.ValidFrom <= date
&& (t.ValidTo == null || t.ValidTo >= date));

            // Si une région est spécifiée, on cherche d'abord une règle spécifique à cette région
            if (!string.IsNullOrEmpty(region))
            {
                var regionSpecificRule = await query
                    .Where(t => t.Region == region)
                    .OrderByDescending(t => t.ValidFrom)
                    .FirstOrDefaultAsync(cancellationToken);

                if (regionSpecificRule != null)
                    return regionSpecificRule;
            }

            // Sinon, on prend la règle nationale (sans région spécifique)
            return await query
                .Where(t => t.Region == null)
                .OrderByDescending(t => t.ValidFrom)
                .FirstOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        /// Récupère toutes les règles de taxe actives
        /// </summary>
        public async Task<List<TaxRule>> GetActiveTaxRulesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.TaxRules
                .Where(t => t.IsActive && !t.IsDeleted)
                .OrderBy(t => t.Country)
                .ThenBy(t => t.Region)
                .ThenByDescending(t => t.ValidFrom)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Récupère les règles de taxe par pays
        /// </summary>
        public async Task<List<TaxRule>> GetTaxRulesByCountryAsync(string country, CancellationToken cancellationToken = default)
        {
            return await _context.TaxRules
                .Where(t => t.Country == country && !t.IsDeleted)
                .OrderByDescending(t => t.ValidFrom)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Récupère une règle de taxe par son code
        /// </summary>
        public async Task<TaxRule?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
        {
            return await _context.TaxRules
                .FirstOrDefaultAsync(t => t.Code == code && !t.IsDeleted, cancellationToken);
        }

        /// <summary>
        /// Vérifie si une règle de taxe est applicable pour une date donnée
        /// </summary>
        public async Task<bool> IsApplicableAsync(Guid taxRuleId, DateTime date, CancellationToken cancellationToken = default)
        {
            var taxRule = await GetByIdAsync(taxRuleId, cancellationToken);
            if (taxRule == null)
                return false;

            return taxRule.IsValidAt(date);
        }

        /// <summary>
        /// Récupère le taux de taxe applicable pour un pays, une date et éventuellement un type de marchandise
        /// </summary>
        public async Task<decimal> GetApplicableTaxRateAsync(string country, DateTime date, Guid? merchandiseTypeId = null, CancellationToken cancellationToken = default)
        {
            var taxRule = await GetApplicableTaxRuleAsync(country, date, cancellationToken);
            if (taxRule == null)
                return 0; // Pas de taxe applicable

            // Si un type de marchandise est spécifié, on pourrait appliquer une logique de taux réduit
            // Pour l'instant, on retourne le taux standard
            return taxRule.StandardRate;
        }
    }
}
