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

        public async Task<PagedList<Payment>> GetAllWithTypesAsync(int? pageNumber, int? pageSize, CancellationToken cancellationToken)
        {
            var query = await _context.Payments
                                        .AsQueryable()
                                        .AsNoTracking()
                                        .Where(e => e.IsDeleted == false)
                                        .ToListAsync(cancellationToken);
            int totalRows = query.AsEnumerable().Count();
            var customer = query
           .Skip(pageNumber.HasValue ? (pageNumber.Value - 1) * pageSize.GetValueOrDefault(1) : 0)
           .Take(pageSize.GetValueOrDefault(int.MaxValue)).ToList();

            return new PagedList<Payment>(customer, totalRows, pageNumber, pageSize);
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
