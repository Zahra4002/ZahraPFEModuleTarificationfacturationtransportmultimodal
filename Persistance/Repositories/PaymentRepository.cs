using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces;
using Domain.Common;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistance.Data;

namespace Persistance.Repositories
{
    public class PaymentRepository : GenericRepository<Payment>, IPaymentRepository
    {
        public PaymentRepository(CleanArchitecturContext context) : base(context)
        {
        }

        // ✅ AJOUTER: GetByIdAsync avec chargement des relations - OVERRIDE pour charger Invoice + Client
        public virtual Task<Payment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return _context.Payments
                .AsNoTracking()
                .Include(p => p.Invoice)
                    .ThenInclude(i => i.Client)  // Charger le client via l'invoice
                .FirstOrDefaultAsync(p => p.Id == id && p.IsDeleted == false, cancellationToken);
        }

        public async Task<PagedList<Payment>> GetAllWithTypesAsync(int? pageNumber, int? pageSize, CancellationToken cancellationToken)
        {
            var query = _context.Payments
                .AsNoTracking()
                .Include(p => p.Invoice)
                    .ThenInclude(i => i.Client)  // ✅ Charger le client
                .Where(e => e.IsDeleted == false)
                .AsQueryable();

            int totalRows = await query.CountAsync(cancellationToken);
            
            var payments = await query
                .OrderByDescending(p => p.PaymentDate)
                .Skip(pageNumber.HasValue ? (pageNumber.Value - 1) * pageSize.GetValueOrDefault(10) : 0)
                .Take(pageSize.GetValueOrDefault(int.MaxValue))
                .ToListAsync(cancellationToken);

            return new PagedList<Payment>(payments, totalRows, pageNumber, pageSize);
        }
        public async Task<IReadOnlyList<Payment>> GetByInvoiceAsync(string invoiceNumber, CancellationToken ct = default)
        {
            return await _context.Payments
                .AsNoTracking()
                .Where(p => p.InvoiceId.ToString() == invoiceNumber)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync(ct);
        }
        public async Task UpdateAsync(Payment entity, CancellationToken ct = default)
        {
            _context.Payments.Update(entity);
            await _context.SaveChangesAsync(ct);
        }

        public async Task SaveChangesAsync(CancellationToken ct = default)
        {
            await _context.SaveChangesAsync(ct);
        }

    }
}
