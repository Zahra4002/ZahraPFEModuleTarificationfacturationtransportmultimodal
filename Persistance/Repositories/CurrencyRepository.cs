using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistance.Data;
using System.Linq.Expressions;

namespace Persistance.Repositories
{
    public class CurrencyRepository : GenericRepository<Currency>, ICurrencyRepository
    {
        public CurrencyRepository(CleanArchitecturContext context) : base(context)
        {
        }

        public Task<ExchangeRate> AddExchangeRate(ExchangeRate exchangeRate)
        {
            _context.ExchangeRates.Add(exchangeRate);

            return Task.FromResult(exchangeRate);

        }

        public async Task<ExchangeRate?> ConvertAmount(string FromCurrencyCode, string ToCurrencyCode, decimal amount, DateTime date, CancellationToken cancellationToken)
        {
            var query = _context.ExchangeRates
                    .Include(rate => rate.FromCurrency)
                    .Include(rate => rate.ToCurrency)
                    .Where(rate => rate.FromCurrency.Code == FromCurrencyCode
                        && rate.ToCurrency.Code == ToCurrencyCode
                        && rate.EffectiveDate <= date
                        && (!rate.ExpiryDate.HasValue || rate.ExpiryDate.Value >= date))
                    .OrderByDescending(rate => rate.EffectiveDate);

            return await query.FirstOrDefaultAsync(cancellationToken);
        }



        public async Task<List<ExchangeRate>> GetRatesByFromCodeAndToCode(
            string fromCurrencyCode,
            string toCurrencyCode,
            DateTime? dateFrom,
            DateTime? dateTo,
            CancellationToken cancellationToken)
        {
            var query = _context.ExchangeRates
                .Include(rate => rate.FromCurrency)
                .Include(rate => rate.ToCurrency)
                .Where(rate => rate.FromCurrency.Code == fromCurrencyCode && rate.ToCurrency.Code == toCurrencyCode);

            if (dateFrom.HasValue)
            {
                query = query.Where(rate => rate.EffectiveDate >= dateFrom.Value);
            }

            if (dateTo.HasValue)
            {
                query = query.Where(rate => !rate.ExpiryDate.HasValue || rate.ExpiryDate.Value >= dateTo.Value);
            }

            return await query.ToListAsync(cancellationToken);
        }

        public async Task<List<Currency>> SelectManyAsync(Expression<Func<Currency, bool>> predicate, CancellationToken cancellationToken)
        {
            return await _context.Currencies.Where(predicate).ToListAsync();
        }

        public async Task<Currency?> SelectOneAsync(Expression<Func<Currency, bool>> predicate, CancellationToken cancellationToken)
        {
            return await _context.Currencies.Where(predicate).FirstOrDefaultAsync();
        }
    }
}