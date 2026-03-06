using Application.Interfaces;
using Domain.Common;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistance.Data;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Persistance.Repositories
{
    public class QuoteRepository : GenericRepository<Quote>, IQuoteRepository
    {
        private readonly CleanArchitecturContext _context;

        public QuoteRepository(CleanArchitecturContext context) : base(context)
        {
            _context = context;
        }

        // ✅ Méthodes CRUD de base
        public async Task AddAsync(Quote quote, CancellationToken cancellationToken = default)
        {
            await _context.Quotes.AddAsync(quote, cancellationToken);
        }

        public async Task UpdateAsync(Quote quote, CancellationToken cancellationToken = default)
        {
            _context.Quotes.Update(quote);
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(Quote quote, CancellationToken cancellationToken = default)
        {
            _context.Quotes.Remove(quote);
            await Task.CompletedTask;
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<Quote?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Quotes
                .FirstOrDefaultAsync(q => q.Id == id && !q.IsDeleted, cancellationToken);
        }

        // ✅ Méthodes avec détails
        public async Task<PagedList<Quote>> GetAllWithDetailsAsync(
            int? pageNumber,
            int? pageSize,
            CancellationToken cancellationToken)
        {
            var query = _context.Quotes
                .AsNoTracking()
                .Where(q => !q.IsDeleted)
                .Include(q => q.Client)
                .Include(q => q.MerchandiseType)
                .OrderByDescending(q => q.CreatedDate)
                .AsQueryable();

            var totalRows = await query.CountAsync(cancellationToken);

            var quotes = await query
                .Skip((pageNumber.GetValueOrDefault(1) - 1) * pageSize.GetValueOrDefault(10))
                .Take(pageSize.GetValueOrDefault(10))
                .ToListAsync(cancellationToken);

            return new PagedList<Quote>(
                quotes,
                totalRows,
                pageNumber,
                pageSize);
        }

        public async Task<Quote?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _context.Quotes
                .AsNoTracking()
                .Where(q => !q.IsDeleted && q.Id == id)
                .Include(q => q.Client)
                .Include(q => q.MerchandiseType)
                .Include(q => q.Shipment)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<Quote?> GetByQuoteNumberAsync(string quoteNumber)
        {
            return await _context.Quotes
                .FirstOrDefaultAsync(q => q.QuoteNumber == quoteNumber && !q.IsDeleted);
        }
        public async Task<List<Quote>> GetByClientIdAsync(Guid clientId, CancellationToken cancellationToken = default)
        {
            return await _context.Quotes
                .AsNoTracking()
                .Where(q => !q.IsDeleted && q.ClientId == clientId)
                .Include(q => q.Client)
                .Include(q => q.MerchandiseType)
                .Include(q => q.Shipment)
                .OrderByDescending(q => q.CreatedDate)
                .ToListAsync(cancellationToken);
        }
    }
}