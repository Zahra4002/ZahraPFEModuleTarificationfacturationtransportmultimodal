using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistance.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Persistance.Repositories
{
    public class InvoiceLineRepository : GenericRepository<InvoiceLine>, IInvoiceLineRepository
    {
        private readonly CleanArchitecturContext _context;

        public InvoiceLineRepository(CleanArchitecturContext context) : base(context)
        {
            _context = context;
        }

        // ✅ Ajouter AddAsync
        public async Task AddAsync(InvoiceLine line)
        {
            await _context.InvoiceLines.AddAsync(line);
        }

        // ✅ Ajouter SaveChangesAsync
        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }

        // ✅ Ajouter GetByIdWithDetailsAsync
        public async Task<InvoiceLine?> GetByIdWithDetailsAsync(Guid id)
        {
            return await _context.InvoiceLines
                .Include(l => l.Invoice)
                .Include(l => l.TransportSegment)
                .FirstOrDefaultAsync(l => l.Id == id);
        }

        public async Task<List<InvoiceLine>> GetByInvoiceIdAsync(Guid invoiceId)
        {
            return await _context.InvoiceLines
                .Where(l => l.InvoiceId == invoiceId)
                .ToListAsync();
        }

        public async Task DeleteByInvoiceIdAsync(Guid invoiceId)
        {
            var lines = await GetByInvoiceIdAsync(invoiceId);
            _context.InvoiceLines.RemoveRange(lines);
            await Task.CompletedTask;
        }
        public async Task<InvoiceLine?> GetByIdAsync(Guid id)
        {
            return await _context.InvoiceLines
                .FirstOrDefaultAsync(l => l.Id == id);
        }

        public async Task UpdateAsync(InvoiceLine line)
        {
            _context.InvoiceLines.Update(line);
            await Task.CompletedTask;
        }
        public async Task<Invoice?> GetByIdWithLinesAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _context.Invoices
                .Include(x => x.Lines)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }
        public async Task DeleteAsync(InvoiceLine line)
        {
            _context.InvoiceLines.Remove(line);
            await Task.CompletedTask;
        }


    }
}