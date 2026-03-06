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
    public class SupplierRepository : GenericRepository<Supplier>, ISupplierRepository
    {
        private readonly CleanArchitecturContext _context;

        public SupplierRepository(CleanArchitecturContext context) : base(context)
        {
            _context = context;
        }

        public async Task AddAsync(Supplier supplier, CancellationToken cancellationToken = default)
        {
            await _context.Suppliers.AddAsync(supplier, cancellationToken);
        }

        public async Task UpdateAsync(Supplier supplier, CancellationToken cancellationToken = default)
        {
            _context.Suppliers.Update(supplier);
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(Supplier supplier, CancellationToken cancellationToken = default)
        {
            _context.Suppliers.Remove(supplier);
            await Task.CompletedTask;
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<Supplier?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Suppliers
                .FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted, cancellationToken);
        }

        public async Task<Supplier?> GetByCodeAsync(string code)
        {
            return await _context.Suppliers
                .FirstOrDefaultAsync(s => s.Code == code && !s.IsDeleted);
        }

        public async Task<PagedList<Supplier>> GetAllWithDetailsAsync(
            int? pageNumber,
            int? pageSize,
            CancellationToken cancellationToken)
        {
            var query = _context.Suppliers
                .AsNoTracking()
                .Where(s => !s.IsDeleted)
                .Include(s => s.Contracts)
                .Include(s => s.TransportSegments)
                    .ThenInclude(ts => ts.ZoneFromet)
                .Include(s => s.TransportSegments)
                    .ThenInclude(ts => ts.ZoneTo)
                .OrderBy(s => s.Code)
                .AsQueryable();

            var totalRows = await query.CountAsync(cancellationToken);

            var suppliers = await query
                .Skip((pageNumber.GetValueOrDefault(1) - 1) * pageSize.GetValueOrDefault(10))
                .Take(pageSize.GetValueOrDefault(10))
                .ToListAsync(cancellationToken);

            return new PagedList<Supplier>(
                suppliers,
                totalRows,
                pageNumber,
                pageSize);
        }

        public async Task<Supplier?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _context.Suppliers
                .AsNoTracking()
                .Where(s => !s.IsDeleted && s.Id == id)
                .Include(s => s.Contracts)
                .Include(s => s.TransportSegments)
                    .ThenInclude(ts => ts.ZoneFromet)
                .Include(s => s.TransportSegments)
                    .ThenInclude(ts => ts.ZoneTo)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<bool> HasContractsAsync(Guid supplierId)
        {
            return await _context.Contracts
                .AnyAsync(c => c.SupplierId == supplierId && !c.IsDeleted);
        }
        public async Task<Supplier?> GetByCodeWithDetailsAsync(string code, CancellationToken cancellationToken = default)
        {
            return await _context.Suppliers
                .AsNoTracking()
                .Where(s => !s.IsDeleted && s.Code == code)
                .Include(s => s.Contracts)
                .Include(s => s.TransportSegments)
                    .ThenInclude(ts => ts.ZoneFromet)
                .Include(s => s.TransportSegments)
                    .ThenInclude(ts => ts.ZoneTo)
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}