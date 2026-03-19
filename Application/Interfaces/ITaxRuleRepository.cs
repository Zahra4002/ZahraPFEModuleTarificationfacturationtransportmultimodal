using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface ITaxRuleRepository : IGenericRepository<TaxRule>
    {
        /// <summary>
        /// Récupère la règle de taxe applicable pour un pays et une date donnés
        /// </summary>
        /// <param name="country">Code du pays (FR, TN, etc.)</param>
        /// <param name="date">Date effective</param>
        /// <param name="cancellationToken">Token d'annulation</param>
        /// <returns>La règle de taxe applicable ou null si aucune</returns>
        Task<TaxRule?> GetApplicableTaxRuleAsync(string country, DateTime date, CancellationToken cancellationToken = default);
        /// <summary>
        /// Récupère la règle de taxe applicable pour un pays, une région et une date donnés
        /// </summary>
        /// <param name="country">Code du pays</param>
        /// <param name="region">Région (optionnel)</param>
        /// <param name="date">Date effective</param>
        /// <param name="cancellationToken">Token d'annulation</param>
        /// <returns>La règle de taxe applicable ou null si aucune</returns>
        Task<TaxRule?> GetApplicableTaxRuleAsync(string country, string? region, DateTime date, CancellationToken cancellationToken = default);
        /// <summary>
        /// Récupère toutes les règles de taxe actives
        /// </summary>
        Task<List<TaxRule>> GetActiveTaxRulesAsync(CancellationToken cancellationToken = default);
        /// <summary>
        /// Récupère les règles de taxe par pays
        /// </summary>
        Task<List<TaxRule>> GetTaxRulesByCountryAsync(string country, CancellationToken cancellationToken = default);
        /// <summary>
        /// Récupère une règle de taxe par son code
        /// </summary>
        Task<TaxRule?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    }
}

