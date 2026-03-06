using Application.Interfaces;
using Domain.Common;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Persistance.Data;

namespace Persistance.Repositories
{
    public class InvoiceRepository
        : GenericRepository<Invoice>, IInvoiceRepository
    {
        public InvoiceRepository(CleanArchitecturContext context)
            : base(context)
        {
        }

        // ==========================================
        // GET ALL WITH DETAILS + PAGINATION
        // ==========================================
        public async Task<PagedList<Invoice>> GetAllWithDetailsAsync(
            int? pageNumber,
            int? pageSize,
            CancellationToken cancellationToken)
        {
            var query = _context.Invoices
                .AsNoTracking()
                .Where(i => i.IsDeleted == false)
                .Include(i => i.Client)
                .Include(i => i.Lines)
                .Include(i => i.Payements)
                .Include(i => i.Currency)
                .AsQueryable();

            var totalRows = await query.CountAsync(cancellationToken);

            var invoices = await query
                .OrderByDescending(i => i.IssueDate)
                .Skip(pageNumber.HasValue
                        ? (pageNumber.Value - 1) * pageSize.GetValueOrDefault(10)
                        : 0)
                .Take(pageSize.GetValueOrDefault(int.MaxValue))
                .ToListAsync(cancellationToken);

            return new PagedList<Invoice>(
                invoices,
                totalRows,
                pageNumber,
                pageSize);
        }

        // ==========================================
        // GET BY ID WITH DETAILS
        // ==========================================
        public async Task<Invoice?> GetByIdWithDetailsAsync(
            Guid id,
            CancellationToken cancellationToken)
        {
            return await _context.Invoices
                .AsNoTracking()
                .Where(i => i.IsDeleted == false && i.Id == id)
                .Include(i => i.Client)
                .Include(i => i.Lines)
                .Include(i => i.Payements)
                .Include(i => i.Currency)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<List<Invoice>> GetOverdueInvoicesAsync()
        {
            return await _context.Invoices
                .Include(i => i.Client)
                .Include(i => i.Supplier)
                .Include(i => i.Shipment)
                .Include(i => i.Currency)
                .Where(i => i.Status == InvoiceStatus.Envoyee
                            && i.DueDate < DateTime.UtcNow
                            && (i.TotalTTC - i.AmountPaid) > 0)
                .ToListAsync();
        }

        public async Task<Invoice?> GetByShipmentIdAsync(Guid shipmentId)
        {
            return await _context.Invoices
                .FirstOrDefaultAsync(i => i.ShipmentId == shipmentId);
        }

        public async Task<Invoice?> GetLastInvoiceAsync()
        {
            return await _context.Invoices
                .OrderByDescending(i => i.CreatedDate)
                .FirstOrDefaultAsync();
        }
        // ✅ AJOUTER AddAsync
        public async Task AddAsync(Invoice invoice)
        {
            await _context.Invoices.AddAsync(invoice);
        }

        // ✅ AJOUTER SaveChangesAsync
        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }

        // ✅ AJOUTER GetByShipmentIdAsync (si manquant)
        public async Task<Invoice?> GetByIdAsync(Guid id)

        {

            return await _context.Invoices
                .Include(i => i.Client)
                .Include(i => i.Supplier)
                .Include(i => i.Shipment)
                .Include(i => i.Currency)
                .Include(i => i.Lines)
                .FirstOrDefaultAsync(i => i.Id == id);

        }

        public async Task UpdateAsync(Invoice invoice)
        {
            _context.Invoices.Update(invoice);
            await Task.CompletedTask;
        }

        public async Task<Invoice?> GetByIdWithDetailsAsync(Guid id)
        {
            return await _context.Invoices
                
                .Include(i => i.Client)
                .Include(i => i.Lines)
                .Include(i => i.Shipment)
                .Include(i => i.Supplier)
                .Include(i => i.Currency)
                .FirstOrDefaultAsync(i => i.Id == id);
        }
        public async Task<Invoice?> GetByIdWithLinesAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _context.Invoices
                .Include(x => x.Lines)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        }

        public async Task<List<Invoice>> GetInvoicesByDateRangeAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default)
        {
            return await _context.Invoices
                .AsNoTracking()
                .Where(i => !i.IsDeleted
                            && i.InvoiceDate >= from
                            && i.InvoiceDate <= to
                            && (i.Status == InvoiceStatus.Payee
                                || i.Status == InvoiceStatus.Envoyee
                                || i.Status == InvoiceStatus.Emise))
                .Include(i => i.Client)
                .Include(i => i.Lines)
                .ToListAsync(cancellationToken);
        }
        public async Task<List<Invoice>> GetInvoicesWithClientsByDateRangeAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default)
        {
            return await _context.Invoices
                .AsNoTracking()
                .Where(i => !i.IsDeleted
                            && i.InvoiceDate >= from
                            && i.InvoiceDate <= to
                            && (i.Status == InvoiceStatus.Payee
                                || i.Status == InvoiceStatus.Envoyee
                                || i.Status == InvoiceStatus.Emise))
                .Include(i => i.Client)
                .ToListAsync(cancellationToken);
        }
        public async Task<List<Invoice>> GetInvoicesByDateRangeWithShipmentsAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default)
        {
            return await _context.Invoices
                .AsNoTracking()
                .Where(i => !i.IsDeleted
                            && i.InvoiceDate >= from
                            && i.InvoiceDate <= to
                            && (i.Status == InvoiceStatus.Payee
                                || i.Status == InvoiceStatus.Envoyee))
                .Include(i => i.Shipment)
                .ToListAsync(cancellationToken);
        }
        public async Task<List<Invoice>> GetAllInvoicesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Invoices
                .AsNoTracking()
                .Where(i => !i.IsDeleted)
                .ToListAsync(cancellationToken);
        }
        public async Task<List<Invoice>> GetOverdueInvoicesWithDetailsAsync(DateTime currentDate, CancellationToken cancellationToken = default)
        {
            return await _context.Invoices
                .AsNoTracking()
                .Where(i => !i.IsDeleted
                            && i.Status == InvoiceStatus.Envoyee
                            && i.DueDate < currentDate
                            && i.TotalTTC > i.AmountPaid)
                .Include(i => i.Client)
                .Include(i => i.Supplier)
                .Include(i => i.Shipment)
                .Include(i => i.Currency)
                .OrderBy(i => i.DueDate)
                .ToListAsync(cancellationToken);
        }




    }
}