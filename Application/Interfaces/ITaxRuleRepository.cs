using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface ITaxRuleRepository : IGenericRepository<TaxRule>
    {
        Task<TaxRule?> GetApplicableTaxRuleAsync(string country, DateTime date, CancellationToken cancellationToken = default);
        Task<TaxRule?> GetApplicableTaxRuleAsync(string country, string? region, DateTime date, CancellationToken cancellationToken = default);
        Task<List<TaxRule>> GetActiveTaxRulesAsync(CancellationToken cancellationToken = default);
        Task<List<TaxRule>> GetTaxRulesByCountryAsync(string country, CancellationToken cancellationToken = default);
        Task<TaxRule?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
        Task<TaxRule?> GetByCountryCode(string countryCode, CancellationToken cancellationToken = default);
    }
}