using Domain.Common;
using Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface ISupplierRepository : IGenericRepository<Supplier>
    {
        Task<Supplier?> GetByCodeAsync(string code);
        Task<PagedList<Supplier>> GetAllWithDetailsAsync(int? pageNumber, int? pageSize, CancellationToken cancellationToken);
        Task<Supplier?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken);
        Task<bool> HasContractsAsync(Guid supplierId);
        Task AddAsync(Supplier supplier, CancellationToken cancellationToken = default);
        Task UpdateAsync(Supplier supplier, CancellationToken cancellationToken = default);
        Task DeleteAsync(Supplier supplier, CancellationToken cancellationToken = default);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
        Task<Supplier?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Supplier?> GetByCodeWithDetailsAsync(string code, CancellationToken cancellationToken = default);
    }
}