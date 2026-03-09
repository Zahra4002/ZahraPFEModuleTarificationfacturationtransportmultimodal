using Domain.Entities;
using System.Linq.Expressions;

namespace Application.Interfaces
{
    public interface ICurrencyRepository : IGenericRepository<Currency>
    {

        Task<Currency> SelectOneAsync(Expression<Func<Currency, bool>> predicate, CancellationToken cancellationToken);

        Task<List<Currency>> SelectManyAsync(Expression<Func<Currency, bool>> predicate, CancellationToken cancellationToken);

        Task<List<ExchangeRate>> GetRatesByFromCodeAndToCode(string fromCurrencyCode, string toCurrencyCode, DateTime? dateFrom, DateTime? dataTo, CancellationToken cancellationToken);

        Task<ExchangeRate> AddExchangeRate(ExchangeRate exchangeRate);

        Task<ExchangeRate?> ConvertAmount(string FromCurrencyCode, string ToCurrencyCode, decimal amount, DateTime date, CancellationToken cancellationToken);


    }
}
