using Domain.Common;
using Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IQuoteRepository : IGenericRepository<Quote>
    {
        Task<PagedList<Quote>> GetAllWithDetailsAsync(int? pageNumber, int? pageSize, CancellationToken cancellationToken);
        Task<Quote?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken);
        Task<Quote?> GetByQuoteNumberAsync(string quoteNumber);

        // ✅ Méthodes supplémentaires si non présentes dans IGenericRepository
        Task AddAsync(Quote quote, CancellationToken cancellationToken = default);
        Task UpdateAsync(Quote quote, CancellationToken cancellationToken = default);
        Task DeleteAsync(Quote quote, CancellationToken cancellationToken = default);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
        Task<Quote?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<List<Quote>> GetByClientIdAsync(Guid clientId, CancellationToken cancellationToken = default);
    }
}